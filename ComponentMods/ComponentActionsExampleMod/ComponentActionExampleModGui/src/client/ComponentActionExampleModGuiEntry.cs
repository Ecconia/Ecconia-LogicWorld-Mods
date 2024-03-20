using System;
using ComponentActionExampleModGui.Client.Ex2;
using ComponentActionExampleModGui.Client.Ex3;
using ComponentActionExampleModGui.Client.Ex6;
using EccsLogicWorldAPI.Client.Hooks;
using LogicAPI.Client;
using LogicWorld;

namespace ComponentActionExampleModGui.Client
{
	public class ComponentActionExampleModGuiEntry : ClientMod
	{
		protected override void Initialize()
		{
			WorldHook.worldLoading += () => {
				//This action is in Unity execution scope, errors must be caught manually:
				try
				{
					EditAddSubber.initialize();
					EditPulseByEdit.initialize();
					EditEditableBlock.initialize();
				}
				catch(Exception e)
				{
					Logger.Error("Failed to initialize ComponentActionExampleMod GUI:");
					SceneAndNetworkManager.TriggerErrorScreen(e);
				}
			};
		}
	}
}
