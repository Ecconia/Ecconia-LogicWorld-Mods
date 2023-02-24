using System;
using LICC;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicWorld.BuildingManagement;
using LogicWorld.Interfaces;
using LogicWorld.Physics;
using LogicWorld.Players;

namespace EcconiaCPUServerComponents.Client
{
	public class RTPulserCommand
	{
		[Command("EditRTPulser", Description = "Set the duration that the RTPulser outputs in milliseconds.")]
		public static void editRTPulser(int milliseconds)
		{
			if(milliseconds <= 0 || !double.IsFinite(milliseconds))
			{
				LConsole.WriteLine("Pulse must be a positive number (its milliseconds).");
				return;
			}
			
			//Yeah no clue what these layers are, but I take them all.
			HitInfo info = PlayerCaster.CameraCast(Masks.Default | Masks.Environment | Masks.Structure | Masks.Peg | Masks.PlayerModel);
			ComponentAddress componentAddress = info.cAddress;
			if(componentAddress == null)
			{
				LConsole.WriteLine("Look at an RTPulser to edit it.");
				return;
			}

			IComponentInWorld componentInWorld = Instances.MainWorld.Data.Lookup(componentAddress);
			string type = Instances.MainWorld.ComponentTypes.GetTextID(componentInWorld.Data.Type);
			if(!type.Equals(RTPulser.COMPONENT_TEXT_ID))
			{
				LConsole.WriteLine("Look at an RTPulser to edit it. Not at: " + type);
				return;
			}
			
			//Undo does not work with the current API.
			BuildRequestManager.SendBuildRequestWithoutAddingToUndoStack(new BuildRequest_UpdateComponentCustomData(componentAddress, BitConverter.GetBytes(milliseconds)));
			LConsole.WriteLine("Sent seconds update to server!");
		}
	}
}
