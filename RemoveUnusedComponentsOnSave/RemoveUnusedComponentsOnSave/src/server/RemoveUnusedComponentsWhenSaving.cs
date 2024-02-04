using System.Collections.Generic;
using EccsLogicWorldAPI.Server;
using EccsLogicWorldAPI.Shared.AccessHelper;
using HarmonyLib;
using LogicAPI.Data;
using LogicAPI.Server;
using LogicAPI.Services;
using LogicWorld.SharedCode.Saving;

namespace RemoveUnusedComponentsOnSave.Server
{
	public class RemoveUnusedComponentsWhenSaving : ServerMod
	{
		private static IWorldData worldData;
		
		protected override void Initialize()
		{
			worldData = ServiceGetter.getService<IWorldData>();
			
			var method = Methods.getPrivateStatic(typeof(SaveWriter), "WriteHeaderAndSaveInfo");
			var patchMethod = Methods.getPrivateStatic(typeof(RemoveUnusedComponentsWhenSaving), nameof(patchSaving));
			var harmony = new Harmony("RemoveUnusedComponentsWhenSaving");
			harmony.Patch(method, new HarmonyMethod(patchMethod));
		}
		
		private static void patchSaving(SaveType saveType, ref IReadOnlyDictionary<ushort, string> componentIDsMap)
		{
			if(saveType != SaveType.World)
			{
				return; //Don't dance with this yet.
			}
			
			//Collect component IDs:
			var usedIDs = new HashSet<ushort>();
			foreach(var topLevelComponent in worldData.TopLevelComponents)
			{
				GoOverComponent(topLevelComponent);
			}
			var newComponentIDMap = new Dictionary<ushort, string>();
			foreach(var usedID in usedIDs)
			{
				newComponentIDMap[usedID] = componentIDsMap[usedID];
			}
			componentIDsMap = newComponentIDMap;
			
			void GoOverComponent(ComponentAddress address)
			{
				var component = worldData.Lookup(address);
				usedIDs.Add(component.Data.Type.NumericID);
				foreach(var childComponent in component.EnumerateChildren())
				{
					GoOverComponent(childComponent);
				}
			}
		}
	}
}
