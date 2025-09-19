using System;
using ComponentActionExampleMod.Client.Raw.Ex1;
using ComponentActionExampleMod.Client.Raw.Ex2;
using ComponentActionExampleMod.Client.Raw.Ex3;
using ComponentActionExampleMod.Client.Raw.Ex5;
using ComponentActionExampleMod.Client.Raw.Ex6;
using ComponentActionExampleMod.Client.Raw.Ex7;
using EccsLogicWorldAPI.Client.Hooks;
using LogicAPI.Client;
using LogicWorld;

namespace ComponentActionExampleMod.Client
{
	public class ComponentActionExampleModClient : ClientMod
	{
		protected override void Initialize()
		{
			ColorBlockActionHandler.init();
			SimpleButtonActionHandler.init();
			EditableBlockActionHandler.init();
			DontLookAtMeActionHandler.init();
			
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
					Logger.Error("Failed to initialize ComponentActionExampleMod GUIs:");
					SceneAndNetworkManager.TriggerErrorScreen(e);
				}
			};
		}
	}
}
