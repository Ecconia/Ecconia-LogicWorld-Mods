using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LICC;
using LogicWorld.Interfaces;
using LogicWorld.Interfaces.Subassemblies;
using LogicWorld.Subassemblies;
using LogicWorld.SharedCode.Subassemblies;

namespace RandomDebugCollection.Client.Commands
{
	public static class ClearSubassemblies
	{
		private static readonly Func<IClientSubassemblyManager, SubassemblyCacheDatabase> getSubassemblyCacheDatabase;

		static ClearSubassemblies()
		{
			getSubassemblyCacheDatabase = Delegator.createFieldGetter<IClientSubassemblyManager, SubassemblyCacheDatabase>(Fields.getPrivate(typeof(ClientSubassemblyManager), "SubassemblyCache"));
		}

		[Command("ClearSubassemblies", Description = "Clears all stored subassemblies on the client. Exists because the official command is broken.")]
		public static void clearSubassemblies()
		{
			SubassemblyCacheDatabase cache = getSubassemblyCacheDatabase(Instances.SubassemblyManager);
			if(cache != null)
			{
				int cacheSize = cache.CachedSubassembliesCount;
				cache.ClearCache();
				LConsole.WriteLine("Cleared {0} subassemblies.", cacheSize);
			}
		}
	}
}
