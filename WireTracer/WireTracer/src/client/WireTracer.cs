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
			if(!GameStateInjector.inject(Logger, WireTracerGameState.id, typeof(WireTracerGameState)))
			{
				throw new WireTracerException("Was not able to load mod, as game state could not be injected. Would result in a runtime error on the client if you try to use the tool.");
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
