using System.Collections.Generic;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI.Interfaces;
using LogicWorld.SharedCode.Components;

namespace FlexibleComponentModUsage.client
{
	public class RegisteredComponentManager
	{
		private static readonly Dictionary<string, ComponentInfo> componentRegistryReference;
		private static readonly Dictionary<string, IComponentActionMutationHandler> componentActionHandlersReference;
		
		static RegisteredComponentManager()
		{
			//Setup component registry backdoor:
			componentRegistryReference = Types.checkType<Dictionary<string, ComponentInfo>>(
				Fields.getNonNull(
					Fields.getPrivateStatic(typeof(ComponentRegistry), "AllRegisteredComponents")
				)
			);
			componentActionHandlersReference = Types.checkType<Dictionary<string, IComponentActionMutationHandler>>(
				Fields.getNonNull(
					Fields.getPrivateStatic(typeof(ComponentActionMutationManager), "Handlers")
				)
			);
		}
		
		//Runtime:
		
		//Original component order, required as dictionaries mess up orders. And the order should stay consistent in GUI.
		private int orderIndex;
		private readonly Dictionary<string, int> componentOrderIndex;
		//Backup reference values for restoring everything:
		private readonly Dictionary<string, ComponentInfo> componentRegistryBackup;
		private readonly Dictionary<string, IComponentActionMutationHandler> componentActionHandlersBackup;
		
		public RegisteredComponentManager()
		{
			componentOrderIndex = new Dictionary<string, int>();
			foreach(var key in componentRegistryReference.Keys)
			{
				componentOrderIndex[key] = orderIndex++;
			}
			//Transmit the original components into a local structure.
			componentRegistryBackup = new Dictionary<string, ComponentInfo>(componentRegistryReference);
			componentActionHandlersBackup = new Dictionary<string, IComponentActionMutationHandler>(componentActionHandlersReference);
		}
		
		public void checkForChanges()
		{
			//Transfer possibly changed components into the local storage.
			var newComponents = 0;
			var updatedComponents = 0;
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
			// By the time other modders gonna dynamically add handlers to the component action API (please don't), this mod is hopefully obsolete due to LW making it so.
			// Thus lets not check for changes in the component action API. If you nevertheless intend to dynamically inject stuff there, tell me so that I can fix this mod.
		}
		
		public void adjust(IReadOnlyDictionary<ushort, string> packetComponentIDsMap)
		{
			//Clear all references, to fill them up again:
			componentRegistryReference.Clear();
			componentActionHandlersReference.Clear();
			
			//Check that every component on the server is installed on the client by looking at the backup:
			foreach(var serverID in packetComponentIDsMap.Values)
			{
				if(!componentRegistryBackup.ContainsKey(serverID))
				{
					//Whoops the component demanded by the server does not really exist.
					//Reset the component map instead of adjusting it to the servers expectations.
					FlexibleComponentModUsage.logger.Warn(
						"Server expects component '" + serverID + "' to be installed. " +
						"But it cannot be found in the original client component map. Falling back to full component map."
					);
					restore();
					return;
				}
				
				//Add component handlers:
				if(componentActionHandlersBackup.TryGetValue(serverID, out var handler))
				{
					componentActionHandlersReference[serverID] = handler;
				}
			}
			
			//Adjust components in the official registry to the servers:
			var sortedKeys = new List<string>(packetComponentIDsMap.Values);
			sortedKeys.Sort((a, b) => componentOrderIndex[a].CompareTo(componentOrderIndex[b]));
			foreach(var serverID in sortedKeys)
			{
				//Inject the component, that the server has:
				componentRegistryReference[serverID] = componentRegistryBackup[serverID];
			}
			
			//Done. Now the client thinks it has only the components that the server has.
		}
		
		public void restore()
		{
			componentRegistryReference.Clear();
			foreach(var (clientID, clientInfo) in componentRegistryBackup)
			{
				componentRegistryReference[clientID] = clientInfo;
			}
			componentActionHandlersReference.Clear();
			foreach(var (clientID, clientHandler) in componentActionHandlersBackup)
			{
				componentActionHandlersReference[clientID] = clientHandler;
			}
		}
	}
}
