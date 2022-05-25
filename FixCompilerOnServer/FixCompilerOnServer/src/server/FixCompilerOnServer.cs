using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using LogicAPI.Server;
using LogicLog;
using LogicWorld.SharedCode.Modding.Compilation;
using ServerOnlyMods.Server;

namespace FixCompilerOnServer.server
{
	public class FixCompilerOnServer : ServerMod, IServerSideOnlyMod
	{
		public static ILogicLogger logger;

		protected override void Initialize()
		{
			logger = Logger;
			Logger.Warn("Heads-Up: This mod only works if it is already compiled! So you might have to experience one crash.");
			new Harmony("FixCompilerPatcher").PatchAll();
		}

		//The replacement method:
		public static bool checkAssembly(Assembly assembly)
		{
			return !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location);
		}
	}

	[HarmonyPatch(typeof(ModCompiler))]
	[HarmonyPatch("Compile")]
	public static class PatchDaCompiler
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			bool targetZone = false;
			List<CodeInstruction> instructionsList = new List<CodeInstruction>(instructions);
			for(int i = 0; i < instructionsList.Count; i++)
			{
				CodeInstruction instruction = instructionsList[i];
				if(instruction.operand is MethodBase && ((MethodBase) instruction.operand).Name.Equals("GetAssemblies"))
				{
					targetZone = true;
				}
				if(targetZone)
				{
					if(instruction.opcode == OpCodes.Ldftn)
					{
						targetZone = false;
						MethodInfo replacementMethod = typeof(FixCompilerOnServer).GetMethod("checkAssembly", BindingFlags.Static | BindingFlags.Public);
						if(replacementMethod == null)
						{
							throw new Exception("Fix your code, the replacement method was not found.");
						}
						CodeInstruction replacement = new CodeInstruction(OpCodes.Ldftn, replacementMethod);
						FixCompilerOnServer.logger.Info("Found the bug causing compiler assembly instruction, replacing it!");
						instructionsList[i] = replacement;
					}
				}
			}
			return instructionsList;
		}
	}
}
