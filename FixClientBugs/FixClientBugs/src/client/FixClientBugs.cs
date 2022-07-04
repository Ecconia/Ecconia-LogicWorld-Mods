using System;
using System.Reflection;
using System.Collections.Generic;
using LogicAPI.Client;
using HarmonyLib;

using LogicWorld.Players;

namespace FixClientBugs.Client
{
	public class FixClientBugs : ClientMod
	{
		//Entry point of this mod
		protected override void Initialize()
		{
			//Sets up Harmony and applies the patches
			var harmony = new Harmony("FixClientBugs");
			harmony.PatchAll();
		}
	}

	[HarmonyPatch]
	public class FlyingWithAutoNoclipPatch
	{
		static IEnumerable<MethodBase> TargetMethods()
		{
			Type playerControllerType = typeof(PlayerControllerManager).Assembly.GetType("LogicWorld.Players.Controller.PlayerController", true);
			PropertyInfo flyingProperty = playerControllerType.GetProperty("Flying");
			MethodInfo[] flyingAccessorMethods = flyingProperty.GetAccessors();
			MethodInfo flyingSetter = null;
			foreach(MethodInfo flyingAccessorMethod in flyingAccessorMethods)
			{
				if(flyingAccessorMethod.ReturnType == typeof(void))
				{
					flyingSetter = flyingAccessorMethod;
					break;
				}
			}
			if(flyingSetter == null)
			{
				throw new Exception("Cannot find setter for Flying property of the player. So no fix for your noclip!");
			}
			yield return flyingSetter;
		}

		public static bool Prefix(ref bool ____Flying, ref bool value)
		{
			if(____Flying == value)
			{
				return false;
			}
			return true;
		}
	}
}
