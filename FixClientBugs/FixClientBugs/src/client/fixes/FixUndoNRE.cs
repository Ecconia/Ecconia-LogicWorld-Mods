using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LogicLog;
using LogicWorld.ClientCode;
using LogicWorld.Modding;
using LogicWorld.Rendering.Components;

namespace FixClientBugs.Client.Fixes
{
	public static class FixUndoNRE
	{
		private static readonly MethodBase initMethod = typeof(ComponentClientCode).GetMethod("SetDefaultCustomData", BindingFlags.NonPublic | BindingFlags.Instance);

		public static void init(ILogicLogger logger, Harmony harmony)
		{
			var componentDataTypes = collectComponentDataTypes();
			logger.Info("Found " + componentDataTypes.Count + " components to fix.");
			// foreach(var type in componentDataTypes)
			// {
			// 	logger.Info("- " + type);
			// }
			var hook = typeof(FixUndoNRE).GetMethod(nameof(prefixHook), BindingFlags.Public | BindingFlags.Static);

			foreach(var type in componentDataTypes)
			{
				var specificType = typeof(ComponentClientCode<>).MakeGenericType(type);
				var relevantMethod = specificType.GetMethod("DeserializeData", BindingFlags.Instance | BindingFlags.NonPublic);
				harmony.Patch(relevantMethod, new HarmonyMethod(hook));
			}
		}

		private static List<Type> collectComponentDataTypes()
		{
			var typesToPatch = new List<Type>();
			Type mainTarget = typeof(ComponentClientCode<>);
			collectComponents(Assembly.GetAssembly(typeof(Mount)), mainTarget); //Also process the MHG assembly.
			foreach(var mod in Mods.All)
			{
				if(mod.HasAssembly)
				{
					collectComponents(mod.CodeAssembly, mainTarget);
				}
			}
			return typesToPatch;

			void collectComponents(Assembly assembly, Type target)
			{
				foreach(var type in assembly.GetTypes())
				{
					var baseType = type.BaseType;
					if(baseType != null
						&& baseType.IsGenericType
						&& target.IsAssignableFrom(baseType.GetGenericTypeDefinition())
					)
					{
						if(type.IsGenericType)
						{
							collectComponents(assembly, type);
							continue;
						}
						typesToPatch.Add(baseType.GetGenericArguments()[0]);
					}
				}
			}
		}

		public static bool prefixHook(byte[] data, ComponentClientCode __instance)
		{
			if(data == null)
			{
				initMethod.Invoke(__instance, null);
				return false;
			}
			return true;
		}
	}
}
