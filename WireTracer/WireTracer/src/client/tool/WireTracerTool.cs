using System;
using FancyInput;
using LogicAPI.Data;
using LogicUI;
using LogicUI.MenuTypes;
using LogicWorld.GameStates;
using LogicWorld.Input;
using LogicWorld.Interfaces;
using LogicWorld.Physics;
using LogicWorld.Players;
using LogicWorld.UI.HelpList;
using WireTracer.Client.Keybindings;
using WireTracer.Shared.Packets.C2S;
using WireTracer.Shared.Packets.S2C;

namespace WireTracer.Client.Tool
{
	public static class WireTracerTool
	{
		private static PegAddress initialPegAddress;
		
		private static Guid? currentRequestID;
		private static GenericTracer currentTracer;
		
		public static bool RunFirstPersonClusterHighlighting()
		{
			//Make sure there is no dirty state:
			if(initialPegAddress.IsNotEmpty() || currentTracer != null)
			{
				//For some reason WireTracer did not stop properly.
				WireTracer.logger.Error("Attempted to check for WireTracer activate, but it was not cleanly stopped? Trying to fix this, but you should report this!");
				initialPegAddress = PegAddress.Empty;
				stopCurrentTracer();
			}
			
			//Get the peg (or wire) in question:
			var hitInfo = PlayerCaster.CameraCast(Masks.Environment | Masks.Structure | Masks.Peg | Masks.Wire);
			if(!hitInfo.HitSomething)
			{
				return false;
			}
			//Resolve hit target:
			if(hitInfo.HitPeg)
			{
				initialPegAddress = hitInfo.pAddress;
			}
			else if(hitInfo.HitWire)
			{
				var wireAddress = hitInfo.wAddress;
				var wire = Instances.MainWorld.Data.Lookup(wireAddress);
				//Assume that wire is never null, as we did just ray-casted it.
				initialPegAddress = wire.Point1.IsInputAddress() ? wire.Point1 : wire.Point2;
			}
			else
			{
				return false;
			}
			
			//Got the starting peg, start the tool:
			GameStateManager.TransitionTo(WireTracerGameState.id);
			return true;
		}
		
		public static void onStart()
		{
			//Confirm, that the most important values are set:
			if(initialPegAddress.IsEmpty() || currentTracer != null)
			{
				WireTracer.logger.Error("Started WireTracer tool, without preparing it properly! Initial peg was not set or tracer was still set. Going back to building state.");
				GameStateManager.TransitionBackToBuildingState();
				return;
			}
			
			requestServerHelp();
			
			currentTracer = new LocalTracer(initialPegAddress);
		}
		
		private static void requestServerHelp()
		{
			// Do not "request help" (send a packet to the server), if there is no WireTracer installed:
			if(!WireTracer.isWireTracerSupported())
			{
				return;
			}
			// Do not perform packet overhead sending for a very trivial case:
			if(!initialPegAddress.IsInputAddress())
			{
				var wires = Instances.MainWorld.Data.LookupPegWires(initialPegAddress);
				if(wires == null || wires.Count == 0)
				{
					//Investigating an output peg without any wires. No need to ask the server for help with this.
					// As an output peg is never part of a linking layer nor allows any way of highlighting more than itself.
					return;
				}
			}
			//Send request to server:
			currentRequestID = Guid.NewGuid();
			Instances.SendData.Send(new RequestClusterListing()
			{
				requestGuid = currentRequestID.Value,
				pegAddress = initialPegAddress,
			});
		}
		
		public static void onResponseReceived(ClusterListingResponse response)
		{
			if(!currentRequestID.HasValue || response.requestGuid != currentRequestID.Value)
			{
				//Not matching Guid, old or wrong request, discard.
				return;
			}
			currentRequestID = null; //Received response, clear GUID.
			
			//Clear up all data immediately:
			stopCurrentTracer();
			//Start showing new data:
			currentTracer = new RemoteTracer(response);
		}
		
		public static void onUpdate()
		{
			//It is safer, to just like stop it and then lets re-enter it...
			if(
				CustomInput.DownThisFrame(Trigger.CancelPlacing) //Needed for right click by default
				|| CustomInput.DownThisFrame(UITrigger.Back) //Needed for ESC by default
				|| CustomInput.DownThisFrame(WireTracerTrigger.HighlightCluster)
			)
			{
				GameStateManager.TransitionBackToBuildingState();
			}
			//This should always be possible:
			if(Trigger.ToggleHelp.DownThisFrame())
			{
				ToggleableSingletonMenu<HelpListMenu>.ToggleMenu();
			}
		}
		
		public static void onStop()
		{
			stopCurrentTracer();
			initialPegAddress = PegAddress.Empty;
			currentRequestID = null;
		}
		
		private static void stopCurrentTracer()
		{
			if(currentTracer != null)
			{
				currentTracer.stop();
				currentTracer = null;
			}
		}
	}
}
