using HarmonyLib;
using LogicLog;
using LogicWorld.SharedCode.Modding.Compilation;

namespace RandomDebugCollection.Client
{
	public class PrintCompilationErrors
	{
		public static void Initialize(ILogicLogger logger)
		{
			ErrorPatch.logger = logger;
			new Harmony("RandomDebugCollection.PrintCompileErrors").PatchAll();
		}
	}

	[HarmonyPatch(typeof(ModCompiler), nameof(ModCompiler.Compile))]
	public static class ErrorPatch
	{
		public static ILogicLogger logger;
		private static string lastModName;

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
