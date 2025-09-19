using System;
using EccsLogicWorldAPI.Client.Hooks;
using LogicAPI.Client;
using LogicWorld;
using RadioConnection.Client.EditGUI;

namespace RadioConnection.Client
{
	public class ModClass : ClientMod
	{
		protected override void Initialize()
		{
			WorldHook.worldLoading += () => {
				//This action is in Unity execution scope, errors must be caught manually:
				try
				{
					EditRadioComponent.initialize();
				}
				catch(Exception e)
				{
					Logger.Error("Failed to initialize Radio Component Edit GUI:");
					SceneAndNetworkManager.TriggerErrorScreen(e);
				}
			};
		}
	}
}
