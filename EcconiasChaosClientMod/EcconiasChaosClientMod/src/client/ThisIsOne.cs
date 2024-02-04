using System.Collections.Generic;
using EccsLogicWorldAPI.Client.AccessHelpers;
using LICC;
using LogicAPI.Data.BuildingRequests;
using LogicWorld.ClientCode;
using LogicWorld.Interfaces;
using LogicWorld.Physics;
using LogicWorld.Players;

namespace EcconiasChaosClientMod.Client
{
	public static class ThisIsOne
	{
		[Command("ThisIsOne")]
		public static void thisIsOne()
		{
			var world = Instances.MainWorld;
			if(world == null)
			{
				LConsole.WriteLine("Join a world before using this command.");
				return;
			}
			if(Selection.isMultiSelecting())
			{
				//We have components to investigate!
				var selection = Selection.getCurrentSelection();
				if(selection.Count == 0)
				{
					return; //Whoops, nothing selected yet (probably impossible).
				}
				var undoList = new List<BuildRequest>();
				var delayers = new List<Delayer>();
				foreach(var address in selection)
				{
					var code = world.Renderer.Entities.GetClientCode(address);
					if(code == null || !(code is Delayer delayer))
					{
						continue; //Not a delayer
					}
					var component = world.Data.Lookup(address);
					if(component == null)
					{
						continue; //No such component??
					}
					var currentDelay = delayer.Data.DelayLengthInTicks;
					if(currentDelay == 1)
					{
						continue;
					}
					//Delay is not one!
					delayers.Add(delayer);
					undoList.Add(new BuildRequest_UpdateComponentCustomData(address, component.Data.CustomData));
				}
				if(delayers.Count == 0)
				{
					LConsole.WriteLine("No Delayer with non 1-tick delay selected.");
					return;
				}
				if(!UndoHistory.addToUndoList(undoList))
				{
					return; //Whoops, can't add to undo list, that is unsafe - do not do that.
				}
				foreach(var color in delayers)
				{
					color.Data.DelayLengthInTicks = 1;
					color.Data.DelayCounter = 0; //Better safe than sorry.
				}
				LConsole.WriteLine("Changed " + delayers.Count + " delayers to be 1 tick.");
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
					LConsole.WriteLine("Look at a delayer to change it.");
					return;
				}
				var code = world.Renderer.Entities.GetClientCode(address);
				if(code == null || !(code is Delayer delayer))
				{
					LConsole.WriteLine("That is not a Delayer. Look at one.");
					return;
				}
				if(delayer.Data.DelayLengthInTicks == 1)
				{
					LConsole.WriteLine("Delayer is already on one tick delay.");
					return;
				}
				if(!UndoHistory.addToUndoList(new List<BuildRequest>()
					{
						new BuildRequest_UpdateComponentCustomData(address, component.Data.CustomData),
					}))
				{
					return; //Whoops, can't add to undo list, that is unsafe - do not do that.
				}
				delayer.Data.DelayLengthInTicks = 1;
				delayer.Data.DelayCounter = 0; //Better safe than sorry.
				LConsole.WriteLine("Set Delayer to 1 tick delay.");
				return;
			}
			LConsole.WriteLine("Look at a Delayer to set its delay to 1 tick.");
		}
	}
}
