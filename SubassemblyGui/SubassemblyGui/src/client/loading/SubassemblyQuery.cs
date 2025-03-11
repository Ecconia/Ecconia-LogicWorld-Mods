using System;
using System.Collections.Generic;
using EccsLogicWorldAPI.Shared.AccessHelper;
using JECS;
using LogicWorld.Building.Subassemblies;

namespace SubassemblyGui.Client.loading
{
	public static class SubassemblyQuery
	{
		private static readonly Func<string, string> getMetaPath;
		// private static readonly Func<string, string> getPartialWorldPath;
		
		static SubassemblyQuery()
		{
			getMetaPath = Delegator.createStaticMethodCall<string, string>(
				Methods.getPrivateStatic(typeof(SubassembliesFileSystemManager), "GetSubassemblyMetadataFilePath")
			);
			// getPartialWorldPath = Delegator.createStaticMethodCall<string, string>(
			// 	Methods.getPrivateStatic(typeof(SubassembliesFileSystemManager), "GetSubassemblyPartialWorldFilePath")
			// );
		}
		
		public static List<SubassemblyMeta> gatherSubassemblyMeta()
		{
			var collection = new List<SubassemblyMeta>();
			
			//These locations are guaranteed to have the meta/placement/partialworld files.
			foreach (var subassemblyDirectory in SubassembliesFileSystemManager.GetIDsOfAllSavedSubassemblies())
			{
				var metaPath = getMetaPath(subassemblyDirectory);
				var metadata = new ReadOnlyDataFile(metaPath).GetAsObject<SubassemblyMetadata>();
				
				//Relevant data:
				var title = metadata.Title;
				
				collection.Add(new SubassemblyMeta()
				{
					folder = subassemblyDirectory,
					title = title,
				});
			}
			
			return collection;
		}
	}
}
