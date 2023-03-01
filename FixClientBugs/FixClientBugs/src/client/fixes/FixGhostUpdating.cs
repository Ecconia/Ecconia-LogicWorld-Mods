using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LICC;
using LogicAPI.Data;
using LogicLog;
using LogicWorld.Building;
using LogicWorld.Building.Overhaul;

namespace FixClientBugs.Client.Fixes
{
	//Hot-fixes that wire ghosts which are invisible (and hence might not have been initialized) will get forced to use the not initialized data, by skipping them.
	// Invisible wire ghosts should not get updated, or their pegs should be set instead.
	//Anyway, no crash with this.
	public class FixGhostUpdating
	{
		public static void init(ILogicLogger logger, Harmony harmony)
		{
			var target = typeof(MovingGhostsWireManager).GetMethod(nameof(MovingGhostsWireManager.UpdateWireOutlines), BindingFlags.Public | BindingFlags.Instance);
			var hook = typeof(FixGhostUpdating).GetMethod(nameof(prefixHook), BindingFlags.Public | BindingFlags.Static);

			harmony.Patch(target, new HarmonyMethod(hook));
		}

		private static FieldInfo ghostField;

		public static bool prefixHook(Dictionary<WireAddress, object> ___MovingWireGhosts)
		{
			foreach(object movingWire in ___MovingWireGhosts.Values) {
				if(ghostField == null)
				{
					ghostField = movingWire.GetType().GetField("Ghost", BindingFlags.Public | BindingFlags.Instance);
					if(ghostField == null)
					{
						throw new Exception("Was not able to get the Ghost field in MovingWire - not able to attempt to fix this.");
					}
				}
				WireGhost ghost = (WireGhost) ghostField.GetValue(movingWire);
				if(!ghost.GameObject.activeSelf)
				{
					// LConsole.WriteLine("OH NOES HIDDEN WIRE GHOST!");
					continue;
				}
				ghost.UpdateValidityAndOutline();
			}
			return false; //No need to execute the original!
		}
	}
}
