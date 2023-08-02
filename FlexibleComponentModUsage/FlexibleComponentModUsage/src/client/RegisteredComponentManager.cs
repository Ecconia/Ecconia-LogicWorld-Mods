using System.Collections.Generic;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicWorld.Rendering;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;

namespace FlexibleComponentModUsage.client
{
	public class RegisteredComponentManager
	{
		private static readonly Dictionary<string, ComponentInfo> componentRegistryReference;
		private static readonly List<PrefabVariantInfo> prefabRegistryReference;

		static RegisteredComponentManager()
		{
			//Setup component registry backdoor:
			componentRegistryReference = Types.checkType<Dictionary<string, ComponentInfo>>(
				Fields.getNonNull(
					Fields.getPrivateStatic(typeof(ComponentRegistry), "AllRegisteredComponents")
				)
			);
			prefabRegistryReference = Types.checkType<List<PrefabVariantInfo>>(
				Fields.getNonNull(
					Fields.getPrivateStatic(
						typeof(RenderUpdateManager).Assembly.GetType("LogicWorld.Rendering.Dynamics.ComponentVariantManager"),
						"PrefabVariantInfoInstances"
					)
				)
			);
		}

		//Runtime:

		private int orderIndex;
		private readonly Dictionary<string, int> componentOrderIndex;
		private readonly Dictionary<string, ComponentInfo> componentRegistryBackup;
		private readonly List<PrefabVariantInfo> prefabRegistryBackup;

		public RegisteredComponentManager()
		{
			componentOrderIndex = new Dictionary<string, int>();
			foreach(var key in componentRegistryReference.Keys)
			{
				componentOrderIndex[key] = orderIndex++;
			}
			//Transmit the original components into a local structure.
			componentRegistryBackup = new Dictionary<string, ComponentInfo>(componentRegistryReference);
			prefabRegistryBackup = new List<PrefabVariantInfo>(prefabRegistryReference);
		}

		public void checkForChanges()
		{
			//Transfer possibly changed components into the local storage.
			int newComponents = 0;
			int updatedComponents = 0;
			foreach(var (newID, newInfo) in componentRegistryReference)
			{
				if(componentRegistryBackup.TryGetValue(newID, out var oldInfo))
				{
					//Component is old!
					if(newInfo != oldInfo)
					{
						componentRegistryBackup[newID] = newInfo;
						updatedComponents++;
					}
				}
				else
				{
					//Component is new!
					componentRegistryBackup[newID] = newInfo;
					componentOrderIndex[newID] = orderIndex++; //Just append to the end *shrug* Its runtime added. Will be different on game start anyway.
					newComponents++;
				}
			}
			if(newComponents != 0 || updatedComponents != 0)
			{
				FlexibleComponentModUsage.logger.Info("Client added " + newComponents + " and updated " + updatedComponents + " components, while gameplay.");
			}
		}

		public void adjust(IReadOnlyDictionary<ushort, string> packetComponentIDsMap)
		{
			//Adjust components in the official registry to the servers:
			componentRegistryReference.Clear();
			var sortedKeys = new List<string>(packetComponentIDsMap.Values);
			sortedKeys.Sort((a, b) => componentOrderIndex[a].CompareTo(componentOrderIndex[b]));
			foreach(var serverID in sortedKeys)
			{
				//Check if the server component is installed locally, by looking at the backup:
				if(!componentRegistryBackup.TryGetValue(serverID, out var serverInfo))
				{
					//Whoops the component demanded by the server does not really exist.
					//Reset the component map instead of adjusting it to the servers expectations.
					FlexibleComponentModUsage.logger.Warn(
						"Server expects a component to be installed, that the client does not have." +
						" Will restore original full client component map, instead of reducing it to the servers map.");
					restore();
					return;
				}
				//Inject the component, that the server has:
				componentRegistryReference[serverID] = serverInfo;
			}
			//Adjust prefabs in the official registry to the servers:
			prefabRegistryReference.Clear();
			foreach(var prefab in prefabRegistryBackup)
			{
				if(componentRegistryReference.ContainsKey(prefab.ComponentTextID))
				{
					prefabRegistryReference.Add(prefab);
				}
			}
			//Done. Now the client thinks it has only the components that the server has.
		}

		private void restore()
		{
			componentRegistryReference.Clear();
			foreach(var (clientID, clientInfo) in componentRegistryBackup)
			{
				componentRegistryReference[clientID] = clientInfo;
			}
			prefabRegistryReference.Clear();
			prefabRegistryReference.AddRange(prefabRegistryBackup);
		}
	}
}
