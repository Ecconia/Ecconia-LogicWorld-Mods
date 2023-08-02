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
