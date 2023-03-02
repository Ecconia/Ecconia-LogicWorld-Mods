using System;
using CustomWirePlacer.Client.CWP;
using CustomWirePlacer.Client.CWP.PegDrawing;
using CustomWirePlacer.Client.Windows;
using FancyInput;
using LogicAPI.Client;
using LogicLog;

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
			//Initialize the overlays:
			CWPStatusOverlay.Init();
			CWPHelpOverlay.Init();
			//Initialize keys:
			CustomInput.Register<CWPContext, CWPTrigger>("CustomWirePlacer");

			//Prepare settings window:
			CWPSettingsWindow.init();
		}
	}
}
