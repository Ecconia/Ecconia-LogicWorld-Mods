using System;
using EccsLogicWorldAPI.Client.Hooks;
using LogicAPI.Client;
using LogicWorld;
using SimulationStopwatch.Client.EditGUI;

namespace SimulationStopwatch.Client
{
	public class ModClass : ClientMod
	{
		protected override void Initialize()
		{
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
