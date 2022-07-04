using System;
using System.Reflection;
using HarmonyLib;
using LogicWorld.Players;

namespace FixClientBugs.Client
{
	public static class FixFlyingWithNoclip
	{
		public static void init(Harmony harmony)
		{
			Type playerControllerType = typeof(PlayerControllerManager).Assembly.GetType("LogicWorld.Players.Controller.PlayerController");
			if(playerControllerType == null)
			{
				ModClass.logger.Error("Could not load class 'PlayerController'. Flying with noclip will not be fixed.");
				return;
			}
			PropertyInfo flyingProperty = playerControllerType.GetProperty("Flying");
			if(flyingProperty == null)
			{
				ModClass.logger.Error("Could not load property 'Flying' in class 'PlayerController'. Flying with noclip will not be fixed.");
				return;
			}
			MethodInfo flyingSetter = flyingProperty.GetSetMethod();
			if(flyingSetter == null)
			{
				ModClass.logger.Error("Cannot find setter for 'Flying' property in class 'PlayerController'. Flying with noclip will not be fixed!");
				return;
			}

			MethodInfo patch = typeof(FixFlyingWithNoclip).GetMethod(nameof(noclipFlyFix), BindingFlags.Static | BindingFlags.NonPublic);
			harmony.Patch(flyingSetter, new HarmonyMethod(patch));
		}

		private static bool noclipFlyFix(ref bool ____Flying, ref bool value)
		{
			//Do not continue execution, if the target value is already reached.
			return ____Flying != value;
		}
	}
}
