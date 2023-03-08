using System.Collections.Generic;
using System.Reflection;
using JimmysUnityUtilities;
using LICC;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicWorld.Building.Overhaul;
using LogicWorld.BuildingManagement;
using LogicWorld.ClientCode;
using LogicWorld.GameStates;
using LogicWorld.Interfaces;
using LogicWorld.Physics;
using LogicWorld.Players;
using LogicWorld.UI;

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
			if(MultiSelector.GameStateTextID.Equals(GameStateManager.CurrentStateID))
			{
				//We have components to investigate!
				var selection = extractMultiSelectedObjects();
				if(selection == null)
				{
					return; //Whoops, could not get selection, stop execution.
				}
				List<BuildRequest> undoList = new List<BuildRequest>();
				var colorables = new List<IColorableClientCode>();
				foreach(var address in selection)
				{
					IComponentClientCode code = world.Renderer.Entities.GetClientCode(address);
					if(code == null || !(code is IColorableClientCode colorable))
					{
						continue;
					}
					IComponentInWorld component = world.Data.Lookup(address);
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
				if(!addToUndoList(undoList))
				{
					return; //Whoops, can't add to undo list, that is unsafe - do not do that.
				}
				var black = new Color24(0, 0, 0);
				foreach(var color in colorables)
				{
					color.Color = black;
				}
				LConsole.WriteLine("Colored " + colorables.Count + " components black.");
				return;
			}
			//Try casting:
			HitInfo info = PlayerCaster.CameraCast(Masks.Environment | Masks.Structure | Masks.Peg | Masks.PlayerModel);
			if(info.HitComponent)
			{
				var address = info.cAddress;
				IComponentInWorld component = world.Data.Lookup(address);
				if(component == null)
				{
					LConsole.WriteLine("Look at a component to paint it.");
					return;
				}
				IComponentClientCode code = world.Renderer.Entities.GetClientCode(address);
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
				if(!addToUndoList(new List<BuildRequest>()
					{
						new BuildRequest_UpdateComponentCustomData(address, component.Data.CustomData),
					}))
				{
					return; //Whoops, can't add to undo list, that is unsafe - do not do that.
				}
				colorable.Color = new Color24(0, 0, 0);
				LConsole.WriteLine("Pained component black.");
				return;
			}
			LConsole.WriteLine("Look at a component to paint it.");
		}

		private static bool addToUndoList(IEnumerable<BuildRequest> undoRequests)
		{
			var method = typeof(UndoManager).GetMethod("ReceiptReceivedForUndoableBuildAction", BindingFlags.NonPublic | BindingFlags.Static);
			if(method == null)
			{
				LConsole.WriteLine("Cannot find method 'ReceiptReceivedForUndoableBuildAction' in class 'UndoManager'. Report this issue to the mod maintainer.");
				return false;
			}
			var fakeReceipt = new BuildRequestReceipt()
			{
				ActionSuccessfullyApplied = true,
				RequestsToUndo = undoRequests,
			};
			method.Invoke(null, new object[] {fakeReceipt});
			return true;
		}

		private static ComponentSelection extractMultiSelectedObjects()
		{
			var field = typeof(MultiSelector).GetField("CurrentSelection", BindingFlags.NonPublic | BindingFlags.Static);
			if(field == null)
			{
				LConsole.WriteLine("Could not get selection of components, as the 'CurrentSelection' field could not be found. Report this issue to the mod maintainer.");
				return null;
			}
			var value = field.GetValue(null);
			if(value == null)
			{
				LConsole.WriteLine("Could not get selection of components, as the current selection is 'null'. Report this issue to the mod maintainer.");
				return null;
			}
			if(!(value is ComponentSelection selection))
			{
				LConsole.WriteLine("Could not get selection of components, as the current selection is a weird type '" + value.GetType() + "'. Report this issue to the mod maintainer.");
				return null;
			}
			if(selection.Count == 0)
			{
				LConsole.WriteLine("Could not get selection of components, as nothing is selected? Report this issue to the mod maintainer.");
				return null;
			}
			return selection;
		}
	}
}
