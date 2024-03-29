using System;
using EccsLogicWorldAPI.Client.Hooks;
using FileDumpGui.Client.EditGUI;
using LogicAPI.Client;
using LogicLog;
using LogicWorld;

namespace FileDumpGui.Client
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
