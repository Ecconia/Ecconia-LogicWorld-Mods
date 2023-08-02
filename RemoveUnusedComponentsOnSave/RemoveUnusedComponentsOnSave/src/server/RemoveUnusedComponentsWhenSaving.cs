using System.Collections.Generic;
using System.Reflection;
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
			
			MethodInfo method = Methods.getPrivateStatic(typeof(SaveWriter), "WriteHeaderAndSaveInfo");
			MethodInfo patchMethod = Methods.getPrivateStatic(typeof(RemoveUnusedComponentsWhenSaving), nameof(patchSaving));
			Harmony harmony = new Harmony("RemoveUnusedComponentsWhenSaving");
			harmony.Patch(method, new HarmonyMethod(patchMethod));
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
