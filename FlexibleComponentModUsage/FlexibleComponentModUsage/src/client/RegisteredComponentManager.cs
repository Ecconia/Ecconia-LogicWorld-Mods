using System.Collections.Generic;
using System.Reflection;
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
			{
				var field = typeof(ComponentRegistry).GetField("AllRegisteredComponents", BindingFlags.NonPublic | BindingFlags.Static);
				if(field == null)
				{
					FlexibleComponentModUsage.logger.Error("Did not find field 'AllRegisteredComponents' inside of 'ComponentRegistry'." + FlexibleComponentModUsage.error);
					return;
				}
				var value = field.GetValue(null);
				if(value == null)
				{
					FlexibleComponentModUsage.logger.Error("Value of field 'AllRegisteredComponents' inside of 'ComponentRegistry' was 'null'." + FlexibleComponentModUsage.error);
					return;
				}
				if(!(value is Dictionary<string, ComponentInfo> registry))
				{
					FlexibleComponentModUsage.logger.Error("Value of field 'AllRegisteredComponents' inside of 'ComponentRegistry' was not of type 'Dictionary<string, ComponentInfo>', but: '" + value.GetType() + "'." + FlexibleComponentModUsage.error);
					return;
				}
				componentRegistryReference = registry;
			}
			{
				var type = typeof(RenderUpdateManager).Assembly.GetType("LogicWorld.Rendering.Dynamics.ComponentVariantManager");
				if(type == null)
				{
					FlexibleComponentModUsage.logger.Error("Did not find type 'ComponentVariantManager' inside of same assembly as type 'RenderUpdateManager'." + FlexibleComponentModUsage.error);
					return;
				}
				var field = type.GetField("PrefabVariantInfoInstances", BindingFlags.NonPublic | BindingFlags.Static);
				if(field == null)
				{
					FlexibleComponentModUsage.logger.Error("Did not find field 'PrefabVariantInfoInstances' inside of 'ComponentVariantManager'." + FlexibleComponentModUsage.error);
					return;
				}
				var value = field.GetValue(null);
				if(value == null)
				{
					FlexibleComponentModUsage.logger.Error("Value of field 'PrefabVariantInfoInstances' inside of 'ComponentVariantManager' was 'null'." + FlexibleComponentModUsage.error);
					return;
				}
				if(!(value is List<PrefabVariantInfo> registry))
				{
					FlexibleComponentModUsage.logger.Error("Value of field 'PrefabVariantInfoInstances' inside of 'ComponentVariantManager' was not of type 'List<PrefabVariantInfo>', but: '" + value.GetType() + "'." + FlexibleComponentModUsage.error);
					return;
				}
				prefabRegistryReference = registry;
			}
		}

		public static bool hasInitializationFailed()
		{
			return componentRegistryReference == null || prefabRegistryReference == null;
		}

		//Runtime:

		private readonly Dictionary<string, ComponentInfo> componentRegistryBackup;
		private readonly List<PrefabVariantInfo> prefabRegistryBackup;

		public RegisteredComponentManager()
		{
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
			foreach(var serverID in packetComponentIDsMap.Values)
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
