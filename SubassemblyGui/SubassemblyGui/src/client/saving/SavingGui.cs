using System.IO;
using EccsGuiBuilder.Client.Layouts.Elements;
using EccsGuiBuilder.Client.Layouts.Helper;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicUI.MenuParts;
using LogicUI.MenuTypes;
using LogicWorld;
using LogicWorld.Building.Overhaul;
using LogicWorld.Building.Subassemblies;
using LogicWorld.GameStates;
using LogicWorld.Interfaces;
using SubassemblyGui.Client.loading;
using TMPro;
using UnityEngine;

namespace SubassemblyGui.Client.Saving
{
	public class SavingGui : ToggleableSingletonMenu<SavingGui>, IAssignMyFields
	{
		private static ComponentSelection currentlyEditingComponents;
		
		public static void initialize()
		{
			WS.window("SubassemblyGuiSaveSubassemblyWindow")
				.setLocalizedTitle("SubassemblyGui.SaveSubassemblyWindow")
				.configureContent(content => content
					.layoutVertical()
					.add(WS.inputField
						.injectionKey(nameof(filePathInputField))
						.fixedSize(1000, 80)
						.setPlaceholderLocalizationKey("SubassemblyGui.Gui.FileName.Hint")
						.disableRichText()
					)
					.add(WS.textLine
						.setLocalizationKey("SubassemblyGui.Gui.SubassemblyAlreadyExists")
						.injectionKey(nameof(errorText))
					)
					.add(WS.button
						.add<ButtonLayout>()
						.injectionKey(nameof(saveButton))
						.setLocalizationKey("SubassemblyGui.Gui.SaveButton")
					)
				)
				.add<SavingGui>()
				.build();
			
			OnMenuShown += () =>
			{
				Instance.onShown();
			};
			OnMenuHidden += () =>
			{
				currentlyEditingComponents = null;
			};
		}
		
		public static void startWithSelection(ComponentSelection selection)
		{
			if (selection == null)
			{
				SceneAndNetworkManager.TriggerErrorScreen("Attempted to open SubassemblyGui SavingGui with 'null' component selection");
				return;
			}
			
			if (selection.Count == 0)
			{
				SceneAndNetworkManager.TriggerErrorScreen("Attempted to open SubassemblyGui SavingGui with empty component selection");
				return;
			}
			
			currentlyEditingComponents = selection;
			GameStateManager.TransitionTo(SavingGameState.ID);
		}
		
		//Instance part:
		
		[AssignMe]
		public TMP_InputField filePathInputField;
		
		[AssignMe]
		public HoverButton saveButton;
		
		[AssignMe]
		public GameObject errorText;
		
		public override void Initialize()
		{
			base.Initialize();
			
			saveButton.OnClickEnd += save;
			filePathInputField.onSubmit.AddListener(text =>
			{
				if (string.IsNullOrWhiteSpace(text))
				{
					return;
				}
				save();
			});
			filePathInputField.onValueChanged.AddListener(text =>
			{
				//Also, this code is not using the official API for Subassemblies entirely.
				// As that API has some identity issues with Subassembly ID names and Path...
				// Thus, manually constructing the folder path and checking if it exists.
				var sanitisedFileName = FileUtilities.ValidatedFileName(text);
				var path = SubassembliesFileSystemManager.GetSubassemblyFolderPath(sanitisedFileName);
				var subassemblyExists = Directory.Exists(path);
				//Show warning if subassembly already exists... Besides that do nothing, as that code might be changed by LogicWorld.
				errorText.SetActive(subassemblyExists);
			});
		}
		
		private void onShown()
		{
			//Reset GUI elements:
			filePathInputField.SetTextWithoutNotify("");
			errorText.SetActive(false);
			filePathInputField.ActivateInputField();
		}
		
		private void save()
		{
			var saveFileName = filePathInputField.text;
			
			var metadata = new SubassemblyMetadata
			{
				Title = saveFileName,
			};
			SubassembliesManager.SaveSelectionAsSubassembly(currentlyEditingComponents, metadata, out var idOfSavedSubassembly);
			Instances.Hotbar.AddItem(new SubassemblyHotbarItemData(idOfSavedSubassembly));
			SubassemblyGui.logger.Info($"Saved subassembly as '{idOfSavedSubassembly}'");
			
			LoadingGui.triggerRefreshOnOpen();
			
			GameStateManager.TransitionBackToBuildingState();
		}
	}
}
