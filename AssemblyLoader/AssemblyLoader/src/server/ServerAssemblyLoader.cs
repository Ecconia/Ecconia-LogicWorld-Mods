using System;
using System.IO;
using System.Reflection;
using LogicLog;

namespace AssemblyLoader.Server
{
	//Huge credit to Cheeses CheeseLoader, for figuring out the dirty work.
	public static class ServerAssemblyLoader
	{
		public static void loadAllServerAssemblies(ILogicLogger logger)
		{
			int failedCounter = 0, succeededCounter = 0;
			var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			foreach(var path in Directory.GetFiles(directory))
			{
				if(!path.EndsWith(".dll"))
				{
					continue;
				}
				var assembly_Name = Path.GetFileNameWithoutExtension(path);
				try
				{
					Assembly.Load(assembly_Name);
					succeededCounter++;
				}
				catch(Exception e)
				{
					failedCounter++;
					logger.Trace("Failed to load assembly '" + assembly_Name + "' due to exception '" + e.GetType().Name + "' with message '" + e.Message + "'.");
				}
			}
			if(failedCounter != 0)
			{
				logger.Warn("Failed to load " + failedCounter + " default server assemblies, successfully loaded " + succeededCounter + " assemblies. Loglevel trace prints details.");
			}
		}
	}
}
