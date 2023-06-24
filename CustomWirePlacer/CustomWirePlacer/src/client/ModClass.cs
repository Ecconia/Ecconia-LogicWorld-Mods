using System;
using CustomWirePlacer.Client.CWP;
using CustomWirePlacer.Client.CWP.PegDrawing;
using CustomWirePlacer.Client.Windows;
using EccsLogicWorldAPI.Client.Hooks;
using FancyInput;
using LogicAPI.Client;
using LogicLog;
using LogicWorld;

namespace CustomWirePlacer.Client
{
	public class ModClass : ClientMod
	{
		// public static SimpleSettings settings;
		public static ILogicLogger logger;

		protected override void Initialize()
		{
			logger = Logger;

			if(!GameStateInjector.inject(Logger, GameStatePegDrawing.id, typeof(GameStatePegDrawing)))
			{
				throw new Exception("[CWP] Could not inject CustomWirePlacer's PegDrawing game state, see previous error.");
			}
			if(!GameStateInjector.inject(Logger, CWPGameState.id, typeof(CWPGameState)))
			{
				throw new Exception("[CWP] Could not inject CustomWirePlacer's game state, see previous error.");
			}

			//Hijack the original WirePlacer to do nothing and instead use the custom one.
			Hijacker.hijackWirePlacer();
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
