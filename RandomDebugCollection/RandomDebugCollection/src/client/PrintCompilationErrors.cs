using System.Reflection;
using HarmonyLib;
using LogicLog;
using LogicWorld.SharedCode.Modding.Compilation;

namespace RandomDebugCollection.Client
{
	public class PrintCompilationErrors
	{
		private static ILogicLogger logger;
		private static string lastModName;
		
		public static void Initialize(ILogicLogger logger)
		{
			PrintCompilationErrors.logger = logger;
			var harmony = new Harmony("RandomDebugCollection.PrintCompileErrors");
			var meth = typeof(ModCompiler).GetMethod("Compile", BindingFlags.Public | BindingFlags.Static);
			var pre = typeof(PrintCompilationErrors).GetMethod("Prefix", BindingFlags.Public | BindingFlags.Static);
			var post = typeof(PrintCompilationErrors).GetMethod("Postfix", BindingFlags.Public | BindingFlags.Static);
			harmony.Patch(meth, new HarmonyMethod(pre), new HarmonyMethod(post));
		}
		
		public static void Prefix(string name)
		{
			lastModName = name;
		}
		 
		public static void Postfix(ref ModCompiler.CompileResult __result)
		{
			if(!__result.Success)
			{
				int skippedFalseEntries = 0;
				logger.Error("Failed to compile mod " + lastModName);
				foreach(var error in __result.Errors)
				{
					if(error.Contains("Assuming assembly reference 'mscorlib,"))
					{
						skippedFalseEntries++;
						continue;
					}
					logger.Error("  Error: " + error);
				}
				if(skippedFalseEntries != 0)
				{
					logger.Info(" Skipped " + skippedFalseEntries + " 'mscorelib' warnings.");
				}
			}
		}
	}
}
