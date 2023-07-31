using System;
using EccsLogicWorldAPI.Client.Injectors;
using FancyInput;
using LogicAPI.Client;
using LogicLog;
using UnityEngine.SceneManagement;
using WireTracer.Client.Keybindings;
using WireTracer.Client.Network;
using WireTracer.Client.network;

namespace WireTracer.Client
{
	public class WireTracer : ClientMod
	{
		public static ILogicLogger logger;
		public static bool serverHasWireTracer;

		protected override void Initialize()
		{
			logger = Logger;
			try
			{
				GameStateInjector.inject(WireTracerGameState.id, typeof(WireTracerGameState));
			}
			catch(Exception e)
			{
				throw new Exception("[WireTracer] Failed to inject GameState, see rest of exception.", e);
			}
			RawPacketHandlerInjector.addPacketHandler(new AnnouncementPacketHandler());
			RawPacketHandlerInjector.addPacketHandler(new ClusterListingResponseHandler());
			CustomInput.Register<WireTracerContext, WireTracerTrigger>("WireTracer");
			WireTracerHook.init();

			//When quitting a server, reset the WireTracer availability state flag:
			SceneManager.sceneLoaded += (_, mode) =>
			{
				if(mode == LoadSceneMode.Single)
				{
					logger.Debug("Reset the join-state!");
					serverHasWireTracer = false;
				}
			};
		}
	}
}
