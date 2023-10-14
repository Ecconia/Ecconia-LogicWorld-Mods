using System.Reflection;
using HarmonyLib;
using LogicAPI.Client;
using LogicLog;
using LogicWorld.SharedCode.Modding.Compilation;

namespace PrintModCompilationErrors.Client
{
	public class PrintModCompilationErrors : ClientMod
	{
		private static ILogicLogger logger;
		private static string lastModName;
		
		protected override void Initialize()
		{
			logger = Logger;
			var harmony = new Harmony("PrintModCompilationErrors");
			var meth = typeof(ModCompiler).GetMethod("Compile", BindingFlags.Public | BindingFlags.Static);
			var pre = typeof(PrintModCompilationErrors).GetMethod(nameof(prefix), BindingFlags.Public | BindingFlags.Static);
			var post = typeof(PrintModCompilationErrors).GetMethod(nameof(postfix), BindingFlags.Public | BindingFlags.Static);
			harmony.Patch(meth, new HarmonyMethod(pre), new HarmonyMethod(post));
		}
		
		public static void prefix(string name)
		{
			lastModName = name;
		}
		
		public static void postfix(ModCompiler.CompileResult __result)
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
					logger.Info(" Skipped " + skippedFalseEntries + " 'mscorlib' warnings.");
				}
			}
		}
	}
}
