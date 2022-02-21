//Needed for reflection classes:
using System.Reflection;

//Needed for 'Command', 'LConsole':
using LICC;
//Needed for 'ILogicLogger':
using LogicLog;

//Needed for 'Instances':
using LogicWorld.Interfaces;
//Needed for 'IClientSubassemblyManager':
using LogicWorld.Interfaces.Subassemblies;
//Needed for 'ClientSubassemblyManager':
using LogicWorld.Subassemblies;
//Needed for 'SubassemblyCacheDatabase':
using LogicWorld.SharedCode.Subassemblies;

namespace RandomDebugCollection.Commands
{
	public static class ClearSubassemblies
	{
		private static FieldInfo fieldSubassemblyCacheDatabase;

		public static void Initialize(ILogicLogger logger)
		{
			var type = typeof(ClientSubassemblyManager);
			var field = type.GetField("SubassemblyCache", BindingFlags.Instance | BindingFlags.NonPublic);
			if(field == null)
			{
				logger.Error("[Command: ClearSubassemblies] Could not find field 'SubassemblyCache', this command will not work.");
				return;
			}
			fieldSubassemblyCacheDatabase = field;
		}

		private static bool isPrimed()
		{
			return fieldSubassemblyCacheDatabase != null;
		}

		private static SubassemblyCacheDatabase getSubassemblyCacheDatabase(ClientSubassemblyManager clientSubassemblyManager)
		{
			return (SubassemblyCacheDatabase) fieldSubassemblyCacheDatabase.GetValue(clientSubassemblyManager);
		}

		[Command("ClearSubassemblies", Description = "Clears all stored subassemblies on the client. Exists because the official command is broken.")]
		private static void clearSubassemblies()
		{
			if(!isPrimed())
			{
				LConsole.WriteLine("Error: Cloud not find field 'SubassemblyCache'. Command aborts.", CColor.Red);
				return;
			}
			IClientSubassemblyManager manager = Instances.SubassemblyManager;
			if(manager == null)
			{
				LConsole.WriteLine("Error: 'Instances.SubassemblyManager' returned 'null'. Command aborts.", CColor.Red);
				return;
			}
			if(!(manager is ClientSubassemblyManager))
			{
				LConsole.WriteLine("Error: Expected class 'ClientSubassemblyManager', but got '{0}'. Command aborts.", CColor.Red, manager.GetType());
				return;
			}
			SubassemblyCacheDatabase cache = getSubassemblyCacheDatabase((ClientSubassemblyManager) manager);
			if(cache != null)
			{
				int cacheSize = cache.CachedSubassembliesCount;
				cache.ClearCache();
				LConsole.WriteLine("Cleared {0} subassemblies.", cacheSize);
			}
		}
	}
}
