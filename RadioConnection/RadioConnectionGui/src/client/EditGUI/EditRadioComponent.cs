using EccsGuiBuilder.Client.Components;
using EccsGuiBuilder.Client.Layouts.Controller;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using LogicUI.MenuParts;
using LogicUI.MenuParts.Toggles;
using LogicUI.Palettes;
using LogicWorld.UI;
using RadioConnection.Shared;
using UnityEngine;

namespace RadioConnectionGui.Client.EditGUI
{
	public class EditRadioComponent : EditComponentMenu<IRadioComponentData>, IAssignMyFields
	{
		public static void initialize()
		{
			const float subFontSize = 45f;
			WS.window("RadioConnectionEditRadioComponentWindow")
				.configureContent(content => content
					.vertical(10f, new RectOffset(20, 20, 0, 20), expandHorizontal: true)
					.addContainer("AddressContainer", data => data
						.vertical(expandHorizontal: true)
						.add(WS.textLine.setLocalizationKey("RadioConnection.Gui.RadioComponent.Address"))
						.add(WS.wrap(VanillaStore.genInnerBox)
							.setAlignment(Alignment.TopLeft)
							.setSize(50, 50)
							.vertical(20, new RectOffset(20, 20, 10, 10), anchor: TextAnchor.MiddleLeft, expandHorizontal: true)
							.addContainer("Offset", top => top
								.addAndConfigure<GapListLayout>(layout => {
									layout.horizontalPreset(gapIndex: 0);
								})
								.add(WS.textLine
									.setLocalizationKey("RadioConnection.Gui.RadioComponent.Address.Offset")
									.setFontSize(subFontSize)
								)
								.add(WS.slider
										.setMin(0)
										.setMax(1023)
										.setInterval(1)
										.fixedSize(600, 45) //Min width is defined here...
										.injectionKey(nameof(addressBase))
								)
								.add(WS.help.fixedSize(50, 50).setLocalizationKey("RadioConnection.Gui.RadioComponent.Address.Offset.Help"))
							)
							.addContainer("Pegs", bottom => bottom
								.addAndConfigure<GapListLayout>(layout => {
									layout.horizontalPreset(gapIndex: 0);
								})
								.add(WS.textLine
									.setLocalizationKey("RadioConnection.Gui.RadioComponent.Address.Pegs")
									.setFontSize(subFontSize)
								)
								.add(WS.slider
										.setMin(0)
										.setMax(10)
										.setInterval(1)
										.fixedSize(600, 45) //Min width is defined here...
										.injectionKey(nameof(addressPegs))
								)
								.add(WS.help.fixedSize(50, 50).setLocalizationKey("RadioConnection.Gui.RadioComponent.Address.Pegs.Help"))
							)
						)
					)
					.addContainer("DataContainer", data => data
						.vertical(expandHorizontal: true)
						.add(WS.textLine.setLocalizationKey("RadioConnection.Gui.RadioComponent.Data"))
						.add(WS.wrap(VanillaStore.genInnerBox)
							.setAlignment(Alignment.TopLeft)
							.setSize(50, 50)
							.vertical(20, new RectOffset(20, 20, 10, 10), anchor: TextAnchor.MiddleLeft, expandHorizontal: true)
							.addContainer("Offset", top => top
								.addAndConfigure<GapListLayout>(layout => {
									layout.horizontalPreset(gapIndex: 0);
								})
								.add(WS.textLine
									.setLocalizationKey("RadioConnection.Gui.RadioComponent.Data.Offset")
									.setFontSize(subFontSize)
								)
								.add(WS.slider
										.setMin(0)
										.setMax(63)
										.setInterval(1)
										.fixedSize(600, 45) //Min width is defined here...
										.injectionKey(nameof(dataOffset))
								)
								.add(WS.help.fixedSize(50, 50).setLocalizationKey("RadioConnection.Gui.RadioComponent.Data.Offset.Help"))
							)
							.addContainer("Pegs", bottom => bottom
								.addAndConfigure<GapListLayout>(layout => {
									layout.horizontalPreset(gapIndex: 0);
								})
								.add(WS.textLine
									.setLocalizationKey("RadioConnection.Gui.RadioComponent.Data.Pegs")
									.setFontSize(subFontSize)
								)
								.add(WS.slider
										.setMin(1)
										.setMax(64)
										.setInterval(1)
										.fixedSize(600, 45) //Min width is defined here...
										.injectionKey(nameof(dataPegs))
								)
								.add(WS.help.fixedSize(50, 50).setLocalizationKey("RadioConnection.Gui.RadioComponent.Data.Pegs.Help"))
							)
						)
					)
					.addContainer("LinkContainer", link => link
						.injectionKey(nameof(linkingRow))
						.vertical(20, expandHorizontal: true)
						.addContainer("LinkRowContainer", con => con
							.addAndConfigure<GapListLayout>(layout => {
								layout.horizontalPreset(gapIndex: 0);
							})
							.add(WS.textLine
								.setLocalizationKey("RadioConnection.Gui.RadioComponent.Link")
								.setFontSize(subFontSize)
							)
							.add(WS.toggle.injectionKey(nameof(optimizeLinking)))
							.add(WS.help.fixedSize(50, 50)
								.setLocalizationKey("RadioConnection.Gui.RadioComponent.Link.Help")
								.setColor(PaletteColor.Secondary)
							)
						)
						.add(WS.wrap(VanillaStore.genDivider).fixedSize(5, 4))
					)
					.addContainer("CompactContainer", link => link
						.addAndConfigure<GapListLayout>(layout => {
							layout.horizontalPreset(gapIndex: 0);
						})
						.add(WS.textLine
							.setLocalizationKey("RadioConnection.Gui.RadioComponent.Compact")
							.setFontSize(subFontSize)
						)
						.add(WS.toggle.injectionKey(nameof(compact)))
						.add(WS.help.fixedSize(50, 50)
							.setLocalizationKey("RadioConnection.Gui.RadioComponent.Compact.Help")
							.setColor(PaletteColor.Secondary)
						)
					)
					.addContainer("FlipContainer", link => link
						.addAndConfigure<GapListLayout>(layout => {
							layout.horizontalPreset(gapIndex: 0);
						})
						.add(WS.textLine
							.setLocalizationKey("RadioConnection.Gui.RadioComponent.Flip")
							.setFontSize(subFontSize)
						)
						.add(WS.toggle.injectionKey(nameof(flip)))
						.add(WS.help.fixedSize(50, 50)
							.setLocalizationKey("RadioConnection.Gui.RadioComponent.Flip.Help")
							.setColor(PaletteColor.Secondary)
						)
					)
				)
				.add<EditRadioComponent>()
				.build();
		}
		
		//Instance part:
		
		[AssignMe]
		public InputSlider addressPegs;
		[AssignMe]
		public InputSlider addressBase;
		[AssignMe]
		public InputSlider dataPegs;
		[AssignMe]
		public InputSlider dataOffset;
		[AssignMe]
		public GameObject linkingRow;
		[AssignMe]
		public ToggleSwitch optimizeLinking;
		[AssignMe]
		public ToggleSwitch compact;
		[AssignMe]
		public ToggleSwitch flip;
		
		public override void Initialize()
		{
			base.Initialize();
			
			//Setup events and handlers:
			addressPegs.OnValueChangedInt += newValue => {
				linkingRow.SetActive(newValue != 0);
				foreach(var entry in ComponentsBeingEdited)
				{
					entry.Data.addressPegs = (uint) newValue;
				}
			};
			addressBase.OnValueChangedInt += newValue => {
				foreach(var entry in ComponentsBeingEdited)
				{
					entry.Data.addressBase = (uint) newValue;
				}
			};
			dataPegs.OnValueChangedInt += newValue => {
				foreach(var entry in ComponentsBeingEdited)
				{
					entry.Data.dataPegs = (uint) newValue;
				}
			};
			dataOffset.OnValueChangedInt += newValue => {
				foreach(var entry in ComponentsBeingEdited)
				{
					entry.Data.dataOffset = (uint) newValue;
				}
			};
			optimizeLinking.OnValueChanged += newValue => {
				foreach(var entry in ComponentsBeingEdited)
				{
					entry.Data.useLinkLayer = newValue;
				}
			};
			compact.OnValueChanged += newValue => {
				foreach(var entry in ComponentsBeingEdited)
				{
					entry.Data.compactPegPlacement = newValue;
				}
			};
			flip.OnValueChanged += newValue => {
				foreach(var entry in ComponentsBeingEdited)
				{
					entry.Data.flipped = newValue;
				}
			};
		}
		
		protected override void OnStartEditing()
		{
			var data = FirstComponentBeingEdited.Data;
			addressPegs.SetValueWithoutNotify(data.addressPegs);
			addressBase.SetValueWithoutNotify(data.addressBase);
			dataPegs.SetValueWithoutNotify(data.dataPegs);
			dataOffset.SetValueWithoutNotify(data.dataOffset);
			optimizeLinking.SetValueWithoutNotify(data.useLinkLayer);
			linkingRow.SetActive(data.addressPegs != 0);
			compact.SetValueWithoutNotify(data.compactPegPlacement);
			flip.SetValueWithoutNotify(data.flipped);
		}
	}
}
