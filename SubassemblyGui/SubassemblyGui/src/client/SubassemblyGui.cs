using System;
using EccsLogicWorldAPI.Client.Hooks;
using EccsLogicWorldAPI.Client.Injectors;
using FancyInput;
using LogicAPI.Client;
using LogicLog;
using LogicWorld;
using LogicWorld.GameStates;
using SubassemblyGui.Client.Inputs;
using SubassemblyGui.Client.loading;
using SubassemblyGui.Client.Saving;

namespace SubassemblyGui.Client
{
	public class SubassemblyGui : ClientMod
	{
		public static ILogicLogger logger;
		
		protected override void Initialize()
		{
			logger = Logger;
			
			CustomInput.Register<SubassemblyGuiContext, SubassemblyGuiTriggers>("SubassemblyGui");
			
			GameStateInjector.inject(SavingGameState.ID, typeof(SavingGameState));
			GameStateInjector.inject(LoadingGameState.ID, typeof(LoadingGameState));
			
			WorldHook.worldLoading += () => {
				try
				{
					SavingGui.initialize();
					SubassemblyCard.initialize(); // Must be initialized before LoadingGui!
					LoadingGui.initialize();
				}
				catch(Exception e)
				{
					Logger.Error("Failed to initialize SubassemblyGui windows:");
					SceneAndNetworkManager.TriggerErrorScreen(e);
				}
			};
			
			// Registering keybinding to the building game state:
			FirstPersonInteraction.RegisterBuildingKeybinding(
				SubassemblyGuiTriggers.OpenLoadGui,
				() => GameStateManager.TransitionTo(LoadingGameState.ID)
			);
		}
	}
}
