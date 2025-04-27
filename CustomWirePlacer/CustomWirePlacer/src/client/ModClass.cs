using System;
using CustomWirePlacer.Client.CWP;
using CustomWirePlacer.Client.CWP.PegDrawing;
using CustomWirePlacer.Client.Windows;
using EccsLogicWorldAPI.Client.Hooks;
using EccsLogicWorldAPI.Client.Injectors;
using FancyInput;
using LogicAPI.Client;
using LogicLog;
using LogicWorld;
using LogicWorld.Input;

namespace CustomWirePlacer.Client
{
	public class ModClass : ClientMod
	{
		// public static SimpleSettings settings;
		public static ILogicLogger logger;
		
		protected override void Initialize()
		{
			logger = Logger;
			
			try
			{
				GameStateInjector.inject(GameStatePegDrawing.id, typeof(GameStatePegDrawing));
				GameStateInjector.inject(CWPGameState.id, typeof(CWPGameState));
			}
			catch(Exception e)
			{
				throw new Exception("[CWP] Could not inject CustomWirePlacer game states, see exception.", e);
			}
			
			//Replace the keybinding of the original WirePlacer with the keybinding of this mod.
			// Make sure to keep the same index & keybinding priority to not confuse players if they rely on certain keybindings.
			//TBI: Maybe replace the trigger to "vanilla wire placer" and thus allow players to use both?
			var index = FirstPersonInteraction.InputActionsFull.PotentialInputs.FindIndex(e => e.Trigger == Trigger.DrawWires);
			if (index < 0)
			{
				throw new Exception("[CWP] Could not replace CustomWirePlacer keybinding from building game state, as 'Trigger.DrawWires' was not found.");
			}
			FirstPersonInteraction.InputActionsFull.PotentialInputs[index] = new InputActions.PotentialInput(
				Trigger.DrawWires,
				CWP.CustomWirePlacer.pollStartWirePlacing
			);
			
			//Initialize keys:
			CustomInput.Register<CWPContext, CWPTrigger>("CustomWirePlacer");
			
			CWPSettingsWindow.initOnce(); //Register static window events
			WorldHook.worldLoading += () => {
				try
				{
					//Initialize the overlays:
					CWPStatusOverlay.Init();
					CWPHelpOverlay.Init();
					//Prepare settings window:
					CWPSettingsWindow.init();
				}
				catch(Exception e)
				{
					logger.Error("Failed to initialize CWP GUI:");
					SceneAndNetworkManager.TriggerErrorScreen(e);
				}
			};
			
			WorldHook.worldUnloading += () => {
				CWPHelpOverlay.destroy();
				CWPStatusOverlay.destroy();
			};
		}
	}
}
