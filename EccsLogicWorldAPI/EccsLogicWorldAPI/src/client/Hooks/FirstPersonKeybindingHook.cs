using System;
using System.Collections.Generic;
using EccsLogicWorldAPI.Shared;
using EccsLogicWorldAPI.Shared.AccessHelper;
using FancyInput;
using LogicWorld.Building;
using LogicWorld.GameStates;

namespace EccsLogicWorldAPI.Client.Hooks
{
	public class FirstPersonKeybindingHook
	{
		private static bool isInitialized;
		private static readonly Dictionary<InputTrigger, string> keybindings = new Dictionary<InputTrigger, string>();
		
		public static void registerKeybinding(InputTrigger trigger, string gameStateID)
		{
			initialize();
			keybindings.Add(trigger, gameStateID);
		}
		
		private static void initialize()
		{
			if (isInitialized)
			{
				return;
			}
			isInitialized = true;
			
			try
			{
				HarmonyAtRuntime.init();
			}
			catch(Exception e)
			{
				throw new Exception("[EccLwApi/FirstPersonKeybindingHook] This API feature requires Harmony to be installed. If it is installed, something is broken. See stacktrace for further debugging.", e);
			}
			
			var methodTarget = Methods.getPrivateStatic(typeof(StuffDeleter), "RunFirstPersonWireDeleting");
			var methodHook = Methods.getPrivateStatic(typeof(FirstPersonKeybindingHook), nameof(hook));
			var harmony = HarmonyAtRuntime.getHarmonyInstance(nameof(FirstPersonKeybindingHook));
			HarmonyAtRuntime.patch(harmony, methodTarget, methodHook);
		}
		
		private static bool hook(out bool deletedWire)
		{
			foreach (var keybinding in keybindings)
			{
				if(CustomInput.DownThisFrame(keybinding.Key))
				{
					GameStateManager.TransitionTo(keybinding.Value);
					
					deletedWire = true; //True = Cancel remaining keybinding handling.
					return false; //Prevent execution of the original/further functionality.
				}
			}
			
			deletedWire = false; //Default, might be overwritten. False = Do not cancel remaining keybinding handling.
			return true; //Allow original/further functionality.
		}
	}
}
