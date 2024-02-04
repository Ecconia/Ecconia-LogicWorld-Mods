using System;
using System.Reflection;
using EccsLogicWorldAPI.Shared.AccessHelper;

namespace EccsLogicWorldAPI.Shared
{
	public static class HarmonyAtRuntime
	{
		private static Type harmonyType;
		private static Type harmonyMethodType;
		private static PropertyInfo harmonyPropertyId;
		private static MethodInfo harmonyMethodUnpatchAll;
		private static MethodInfo harmonyPatchMethod;
		
		public static void init()
		{
			if(harmonyPatchMethod != null)
			{
				//We already got things, nothing to initialize anymore.
				return;
			}
			Assembly harmonyAssembly;
			try
			{
				harmonyAssembly = Assemblies.findAssemblyWithName("0Harmony");
			}
			catch(Exception)
			{
				throw new Exception("Attempted to use Harmony functionality of EccsLogicWorldAPI, but Harmony was not loaded. Install a Harmony providing mod like 'HarmonyForLogicWorld'.");
			}
			harmonyType = Types.getType(harmonyAssembly, "HarmonyLib.Harmony");
			harmonyMethodType = Types.getType(harmonyAssembly, "HarmonyLib.HarmonyMethod");
			harmonyPropertyId = harmonyType.GetProperty("id");
			if(harmonyPropertyId == null)
			{
				//Some newer Harmony version changed it to Id instead of id:
				harmonyPropertyId = harmonyType.GetProperty("Id");
				if(harmonyPropertyId == null)
				{
					throw new AccessHelperException("Could not find property '" + "id" + "' or '" + "Id" + "' in class '" + harmonyType.FullName + "'.");
				}
			}
			harmonyMethodUnpatchAll = harmonyType.GetMethod("UnpatchAll", Bindings.publicInst, null, new Type[]
			{
				typeof(string),
			}, null);
			if(harmonyMethodUnpatchAll == null)
			{
				throw new AccessHelperException("Could not find method '" + "UnpatchAll" + "' in class '" + harmonyType.FullName + "'.");
			}
			//As the client still runs on net4, use this getMethod signature:
			harmonyPatchMethod = harmonyType.GetMethod("Patch", Bindings.publicInst, null, new Type[]
			{
				typeof(MethodBase),
				harmonyMethodType,
				harmonyMethodType,
				harmonyMethodType,
				harmonyMethodType,
			}, null);
			if(harmonyPatchMethod == null)
			{
				throw new AccessHelperException("Could not find method '" + "Patch" + "' in assembly '" + harmonyAssembly.FullName + "'.");
			}
		}
		
		public static object getHarmonyInstance(string name)
		{
			init();
			return Types.createInstance(harmonyType, name);
		}
		
		public static void patch(object harmonyInstance, MethodInfo toPatchMethod, MethodInfo prefix = null, MethodInfo postfix = null)
		{
			object prefixMethod = null;
			object postfixMethod = null;
			if(prefix != null)
			{
				prefixMethod = Types.createInstance(harmonyMethodType, prefix);
			}
			if(postfix != null)
			{
				postfixMethod = Types.createInstance(harmonyMethodType, postfix);
			}
			harmonyPatchMethod.Invoke(harmonyInstance, new object[]
			{
				toPatchMethod, prefixMethod, postfixMethod, null, null,
			});
		}
		
		public static void unpatchAll(object harmonyInstance)
		{
			var id = harmonyPropertyId.GetValue(harmonyInstance);
			harmonyMethodUnpatchAll.Invoke(harmonyInstance, new object[]{id});
		}
	}
}
