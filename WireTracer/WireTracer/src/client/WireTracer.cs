using EccsLogicWorldAPI.Client.Injectors;
using EccsLogicWorldAPI.Client.PacketIndexOrdering;
using FancyInput;
using LogicAPI.Client;
using LogicLog;
using LogicWorld;
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
			PacketIndexOrdering.markModAsOptional(GetType().Assembly);
			
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
			serverHasWireTracer ??= PacketIndexOrdering.doesServerSupportPacket(typeof(ClusterListingResponse));
			return serverHasWireTracer.Value;
		}
	}
}
