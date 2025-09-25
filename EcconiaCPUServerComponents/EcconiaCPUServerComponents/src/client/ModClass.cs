using System;
using EcconiaCPUServerComponents.Client.EditGUI;
using EccsLogicWorldAPI.Client.Hooks;
using LogicAPI.Client;
using LogicLog;
using LogicWorld;

namespace EcconiaCPUServerComponents.Client
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
					EditFlatKey.initialize();
					EditRTPulser.initialize();
				}
				catch(Exception e)
				{
					Logger.Error("Failed to initialize Eccs Component Edit GUIs:");
					SceneAndNetworkManager.TriggerErrorScreen(e);
				}
			};
		}
	}
}
