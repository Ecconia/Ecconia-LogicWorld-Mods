using System;
using EccsLogicWorldAPI.Client.Hooks;
using FileDump.Client.EditGUI;
using LogicAPI.Client;
using LogicWorld;

namespace FileDump.Client
{
	public class ModClass : ClientMod
	{
		protected override void Initialize()
		{
			WorldHook.worldLoading += () => {
				//This action is in Unity execution scope, errors must be caught manually:
				try
				{
					EditFileDump.initialize();
				}
				catch(Exception e)
				{
					Logger.Error("Failed to initialize File Dump Edit GUI:");
					SceneAndNetworkManager.TriggerErrorScreen(e);
				}
			};
		}
	}
}
