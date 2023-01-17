using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LogicAPI.Data;
using LogicAPI.Server;
using LogicAPI.Services;
using LogicWorld.Server;
using LogicWorld.SharedCode.Saving;

namespace RemoveUnusedComponentsOnSave.Server
{
	public class RemoveUnusedComponentsWhenSaving : ServerMod
	{
		private static IWorldData worldData;

		protected override void Initialize()
		{
			worldData = Program.Get<IWorldData>();
			if(worldData == null)
			{
				throw new Exception("Could not get 'IWorldData' from server.");
			}

			MethodInfo methSaver = typeof(SaveWriter).GetMethod("WriteHeaderAndSaveInfo", BindingFlags.NonPublic | BindingFlags.Static);
			if(methSaver == null)
			{
				Logger.Error("Could not find method 'WriteHeaderAndSaveInfo' in class 'SaveWriter'.");
				return;
			}
			MethodInfo patchMethod = typeof(RemoveUnusedComponentsWhenSaving).GetMethod(nameof(patchSaving), BindingFlags.NonPublic | BindingFlags.Static);
			Harmony harmony = new Harmony("RemoveUnusedComponentsWhenSaving");
			harmony.Patch(methSaver, new HarmonyMethod(patchMethod));
		}

		private static void patchSaving(SaveType saveType, ref IReadOnlyDictionary<ushort, string> componentIDsMap)
		{
			if(saveType != SaveType.World)
			{
				return; //Don't dance with this yet.
			}

			//Collect component IDs:
			HashSet<ushort> usedIDs = new HashSet<ushort>();
			foreach(ComponentAddress topLevelComponent in worldData.TopLevelComponents)
			{
				GoOverComponent(topLevelComponent);
			}
			Dictionary<ushort, string> newComponentIDMap = new Dictionary<ushort, string>();
			foreach(ushort usedID in usedIDs)
			{
				newComponentIDMap[usedID] = componentIDsMap[usedID];
			}
			componentIDsMap = newComponentIDMap;

			void GoOverComponent(ComponentAddress address)
			{
				IComponentInWorld component = worldData.Lookup(address);
				usedIDs.Add(component.Data.Type.NumericID);
				foreach(ComponentAddress childComponent in component.EnumerateChildren())
				{
					GoOverComponent(childComponent);
				}
			}
		}
	}
}
