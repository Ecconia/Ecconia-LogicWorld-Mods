using System.Collections.Generic;
using LICC;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicWorld.BuildingManagement;
using LogicWorld.Interfaces;

namespace EcconiaCPUServerComponents.Client.commands
{
	public class Migration
	{
		[Command("eccMigrate", Description = "Add missing pegs to old components.")]
		public static void migrateComponents()
		{
			ComponentType typeDisplay;
			try
			{
				typeDisplay = Instances.MainWorld.ComponentTypes.GetComponentType("EcconiaCPUServerComponents.WeirdCustomDisplay");
			}
			catch(KeyNotFoundException e)
			{
				LConsole.WriteLine("Could not find Display type in component-type map, although mod is installed. Complain to the mods developer!");
				return;
			}
			int displaysThatDoNotNeedUpgrade = 0;
			List<ComponentAddress> gatheredDisplays = new List<ComponentAddress>();
			foreach(var entry in Instances.MainWorld.Data.AllComponents)
			{
				ComponentData data = entry.Value.Data;
				if(data.Type == typeDisplay)
				{
					if(data.InputCount == (32 * 4 + 1)) //32 pixels per length, by XY and Blink, plus data peg.
					{
						gatheredDisplays.Add(entry.Key);
					}
					else //Assume there are no broken displays, and the displays here are all good.
					{
						displaysThatDoNotNeedUpgrade++;
					}
				}
			}
			LConsole.WriteLine("Found " + gatheredDisplays.Count + " displays that need updating. And " + displaysThatDoNotNeedUpgrade + " displays that are updated.");
			foreach(var componentAddress in gatheredDisplays)
			{
				BuildRequestManager.SendBuildRequestWithoutAddingToUndoStack(new BuildRequest_ChangeDynamicComponentPegCounts(componentAddress, 32 * 4 + 2, 0));
			}
			LConsole.WriteLine("Sent peg count change request for " + gatheredDisplays.Count + " displays to server!");
		}
	}
}
