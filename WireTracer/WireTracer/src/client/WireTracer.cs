using EccsLogicWorldAPI.Client.Injectors;
using FancyInput;
using LogicAPI.Client;
using LogicLog;
using LogicWorld;
using Shared_Code.Code.Networking;
using UnityEngine.SceneManagement;
using WireTracer.Client.Keybindings;
using WireTracer.Client.Network;
using WireTracer.Client.Tool;
using WireTracer.Shared.Packets.S2C;

namespace WireTracer.Client
{
	public class WireTracer : ClientMod
	{
		public static ILogicLogger logger;
		private static bool? serverHasWireTracer;
		
		protected override void Initialize()
		{
			logger = Logger;
			
			RawPacketHandlerInjector.addPacketHandler(new ClusterListingResponseHandler());
			
			CustomInput.Register<WireTracerContext, WireTracerTrigger>("WireTracer");
			
			FirstPersonInteraction.RegisterBuildingKeybinding(
				WireTracerTrigger.HighlightCluster,
				WireTracerTool.RunFirstPersonClusterHighlighting
			);
			
			//When quitting a server, reset the WireTracer availability state flag:
			SceneManager.sceneLoaded += (_, mode) =>
			{
				if(mode == LoadSceneMode.Single)
				{
					serverHasWireTracer = null;
				}
			};
		}
		
		public static bool isWireTracerSupported()
		{
			serverHasWireTracer ??= PacketManager.TryGetCodeFromType(typeof(ClusterListingResponse), out var _);
			return serverHasWireTracer.Value;
		}
	}
}
