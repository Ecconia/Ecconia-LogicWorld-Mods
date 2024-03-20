using System.Collections.Generic;
using EccsLogicWorldAPI.Client.AccessHelpers;
using JimmysUnityUtilities;
using LICC;
using LogicAPI.Data.BuildingRequests;
using LogicWorld.BuildingManagement;
using LogicWorld.ClientCode;
using LogicWorld.Interfaces;
using LogicWorld.Physics;
using LogicWorld.Players;

namespace EcconiasChaosClientMod.Client
{
	public static class ThisIsBlack
	{
		[Command("ThisIsBlack")]
		public static void thisIsBlack()
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
				var colorables = new List<IColorableClientCode>();
				foreach(var address in selection)
				{
					var code = world.Renderer.Entities.GetClientCode(address);
					if(code == null || !(code is IColorableClientCode colorable))
					{
						continue;
					}
					var component = world.Data.Lookup(address);
					if(component == null)
					{
						continue;
					}
					var currentColor = colorable.Color;
					if(currentColor.r == 0 && currentColor.g == 0 && currentColor.b == 0)
					{
						continue;
					}
					//Color is not black!
					colorables.Add(colorable);
					undoList.Add(new BuildRequest_UpdateComponentCustomData(address, component.Data.CustomData));
				}
				if(colorables.Count == 0)
				{
					LConsole.WriteLine("Everything is already black, or can't be colored.");
					return;
				}
				UndoManager.AddItemToUndoHistory(new UndoRequests() {RequestsToUndo = undoList});
				var black = new Color24(0, 0, 0);
				foreach(var color in colorables)
				{
					color.Color = black;
				}
				LConsole.WriteLine("Colored " + colorables.Count + " components black.");
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
					LConsole.WriteLine("Look at a component to paint it.");
					return;
				}
				var code = world.Renderer.Entities.GetClientCode(address);
				if(code == null || !(code is IColorableClientCode colorable))
				{
					LConsole.WriteLine("Cannot color that component, look at something else.");
					return;
				}
				var currentColor = colorable.Color;
				if(currentColor.r == 0 && currentColor.g == 0 && currentColor.b == 0)
				{
					LConsole.WriteLine("Component is already black.");
					return;
				}
				UndoManager.AddItemToUndoHistory(new UndoRequests() {RequestsToUndo = new List<BuildRequest>()
				{
					new BuildRequest_UpdateComponentCustomData(address, component.Data.CustomData),
				}});
				colorable.Color = new Color24(0, 0, 0);
				LConsole.WriteLine("Pained component black.");
				return;
			}
			LConsole.WriteLine("Look at a component to paint it.");
		}
	}
}
