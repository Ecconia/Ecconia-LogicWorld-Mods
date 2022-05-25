using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LogicAPI.Client;
using LogicWorld.SharedCode.Modding.Compilation;

namespace FixCompilerOnClient.client
{
	public class FixCompilerOnClient : ClientMod
	{
		private static bool compilingAMod;

		protected override void Initialize()
		{
			var harmony = new Harmony("FixLWCompilerClient");

			var compileMethod = typeof(ModCompiler).GetMethod("Compile", BindingFlags.Static | BindingFlags.Public);
			var hookPrefix = typeof(FixCompilerOnClient).GetMethod("compileHookPrefix", BindingFlags.Static | BindingFlags.Public);
			var hookPostfix = typeof(FixCompilerOnClient).GetMethod("compileHookPostfix", BindingFlags.Static | BindingFlags.Public);
			harmony.Patch(compileMethod, new HarmonyMethod(hookPrefix), new HarmonyMethod(hookPostfix));

			var assembliesMeth = typeof(AppDomain).GetMethod("GetAssemblies", BindingFlags.Instance | BindingFlags.Public);
			var assembliesMethHook = typeof(FixCompilerOnClient).GetMethod("runtimeAssemblies", BindingFlags.Static | BindingFlags.Public);
			harmony.Patch(assembliesMeth, null, new HarmonyMethod(assembliesMethHook));
		}

		public static void compileHookPrefix()
		{
			compilingAMod = true;
		}

		public static void compileHookPostfix()
		{
			compilingAMod = false;
		}

		public static void runtimeAssemblies(ref Assembly[] __result)
		{
			if(!compilingAMod)
			{
				return;
			}
			__result = __result.Where(a =>
			{
				if(a.IsDynamic)
				{
					return true;
				}
				return !string.IsNullOrEmpty(a.Location);
			}).ToArray();
		}
	}
}
