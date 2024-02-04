using System;
using EccsLogicWorldAPI.Client.Hooks;
using LogicAPI.Client;
using LogicLog;
using LogicWorld;
using SimulationStopwatchGui.Client.EditGUI;

namespace SimulationStopwatchGui.Client
{
	public class ModClass : ClientMod
	{
		public static ILogicLogger logger;
		
		protected override void Initialize()
		{
			logger = Logger;
			WorldHook.worldLoading += () => {
				//This action is in Unity execution scope, errors must be caught manually:
				try
				{
					EditSimulationStopwatch.initialize();
				}
				catch(Exception e)
				{
					Logger.Error("Failed to initialize Stopwatch Edit GUI:");
					SceneAndNetworkManager.TriggerErrorScreen(e);
				}
			};
		}
	}
}
