using System;
using EccsLogicWorldAPI.Client.Injectors;
using FancyInput;
using LogicAPI.Client;
using LogicLog;
using UnityEngine.SceneManagement;
using WireTracer.Client.injectors;
using WireTracer.Client.Keybindings;
using WireTracer.Client.Network;
using WireTracer.Client.network;
using WireTracer.Shared;

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
			if(!PacketHandlerInjector.injectNewPacketHandler(Logger, new AnnouncementPacketHandler()))
			{
				throw new WireTracerException("Was not able to load mod, as the announcement packet handler could not be injected. This will result in error on joining a server with this mod.");
			}
			if(!PacketHandlerInjector.injectNewPacketHandler(Logger, new ClusterListingResponseHandler()))
			{
				throw new WireTracerException("Was not able to load mod, as the cluster-listing-response packet handler could not be injected. This will result in error on joining a server with this mod.");
			}
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
