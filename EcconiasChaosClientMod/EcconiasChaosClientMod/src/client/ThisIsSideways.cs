using System.Collections.Generic;
using EccsLogicWorldAPI.Client.AccessHelpers;
using LICC;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicWorld.BuildingManagement;
using LogicWorld.Interfaces;
using LogicWorld.Physics;
using LogicWorld.Players;
using UnityEngine;

namespace EcconiasChaosClientMod.Client
{
	public static class ThisIsSideways
	{
		[Command("ThisIsSideways")]
		public static void thisIsSideways()
		{
			var world = Instances.MainWorld;
			if(world == null)
			{
				LConsole.WriteLine("Join a world before using this command.");
				return;
			}
			var andID = Instances.MainWorld.ComponentTypes.GetComponentType("MHG.AndGate");
			var fixVector = new Vector3(0.15f, -0.15f, 0);
			if(Selection.isMultiSelecting())
			{
				//We have components to investigate!
				var selection = Selection.getCurrentSelection();
				if(selection.Count == 0)
				{
					return; //Whoops, nothing selected yet (probably impossible).
				}
				var undoList = new List<BuildRequest>();
				var requests = new List<(ComponentAddress, IComponentInWorld)>();
				foreach(var address in selection)
				{
					var component = world.Data.Lookup(address);
					if(component == null || component.Data.Type != andID)
					{
						continue; //No such component??
					}
					requests.Add((address, component));
					undoList.Add(new BuildRequest_UpdateComponentPositionRotationParent(
						address,
						component.Data.LocalPosition,
						component.Data.LocalRotation,
						component.Data.Parent
					));
				}
				if(requests.Count == 0)
				{
					LConsole.WriteLine("No AND gate selected.");
					return;
				}
				if(!UndoHistory.addToUndoList(undoList))
				{
					return; //Whoops, can't add to undo list, that is unsafe - do not do that.
				}
				foreach(var (address, component) in requests)
				{
					var newRotation = component.Data.LocalRotation * Quaternion.AngleAxis(90, Vector3.forward);
					BuildRequestManager.SendBuildRequestWithoutAddingToUndoStack(new BuildRequest_UpdateComponentPositionRotationParent(
						address,
						component.Data.LocalPosition + (newRotation * fixVector),
						newRotation,
						component.Data.Parent
					));
				}
				LConsole.WriteLine("Flipped " + requests.Count + " AND gates sideways.");
				return;
			}
			//Try casting:
			var info = PlayerCaster.CameraCast(Masks.Environment | Masks.Structure | Masks.Peg | Masks.PlayerModel);
			if(info.HitComponent)
			{
				var address = info.cAddress;
				var component = world.Data.Lookup(address);
				if(component == null)
				{
					LConsole.WriteLine("Look at an AND gate to flip it sideways.");
					return;
				}
				if(component.Data.Type != andID)
				{
					LConsole.WriteLine("That is not an AND gate. Look at one.");
					return;
				}
				if(!UndoHistory.addToUndoList(new List<BuildRequest>()
					{
						new BuildRequest_UpdateComponentPositionRotationParent(
							address,
							component.Data.LocalPosition,
							component.Data.LocalRotation,
							component.Data.Parent
						),
					}))
				{
					return; //Whoops, can't add to undo list, that is unsafe - do not do that.
				}
				var newRotation = component.Data.LocalRotation * Quaternion.AngleAxis(90, Vector3.forward);
				BuildRequestManager.SendBuildRequestWithoutAddingToUndoStack(new BuildRequest_UpdateComponentPositionRotationParent(
					address,
					component.Data.LocalPosition + (newRotation * fixVector),
					newRotation,
					component.Data.Parent
				));
				LConsole.WriteLine("Flipped AND gate sideways.");
				return;
			}
			LConsole.WriteLine("Look at an AND gate to flip it sideways.");
		}
	}
}
