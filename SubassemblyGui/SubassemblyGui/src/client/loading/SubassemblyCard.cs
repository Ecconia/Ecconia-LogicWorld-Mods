using System.Collections.Generic;
using EccsGuiBuilder.Client.Components;
using EccsGuiBuilder.Client.Layouts.Helper;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using EccsGuiBuilder.Client.Wrappers.RootWrappers;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI.Data;
using LogicLocalization;
using LogicUI.MenuParts;
using LogicUI.MenuTypes.Searching;
using LogicUI.Palettes;
using LogicWorld.Interfaces;
using LogicWorld.UnityHacksAndExtensions;
using ThisOtherThing.UI.Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SubassemblyGui.Client.loading
{
	public class SubassemblyCard : UIBehaviour, IAssignMyFields, IInitializable, ISearchableItem
	{
		public static GameObject pattern { private set; get; }
		
		public static void initialize()
		{
			pattern = VanillaStore.genInnerBox;
			VanillaStore._addInternal(pattern); //Change the parent to something "not in the way" or "not root level".
			
			//This part is a bit "uff" as this element is on root-component level the wrapper system cannot be used.
			// Thus, a manual "hacky" means of setting up the rectangle is used:
			var rectangle = pattern.GetComponent<Rectangle>();
			var outlinePalette = pattern.AddComponent<PaletteRectangleOutline>();
			Fields.getPrivate(outlinePalette.GetType(), "Target").SetValue(outlinePalette, rectangle);
			outlinePalette.SetPaletteColor(PaletteColor.Tertiary);
			rectangle.ShapeProperties.DrawOutline = true;
			
			new CanvasWrapper(pattern, "SubassemblyCard")
				.layoutGrowElementHorizontal(elementIndex: IndexHelper.nth(1))
				.add(WS.textLine
					.setLocalizationKey("SubassemblyGui.Gui.SubassemblyCard.Title")
					.injectionKey(nameof(text))
					.configureTMP(tmp => {
						tmp.fontSize = 40;
						tmp.textWrappingMode = TextWrappingModes.PreserveWhitespace;
					})
				)
				.addAndConfigure<HoverButton>(hoverButton => {
					// I do not have any wrapper support for default buttons, thus the graphic has to be set manually.
					Fields.getPrivate(hoverButton.GetType(), "TargetGraphic").SetValue(hoverButton, hoverButton.GetComponent<Graphic>());
					hoverButton.SetPaletteColor(PaletteColor.Secondary);
				})
				.injectionKey(nameof(hoverButton))
				.add<SubassemblyCard>()
				.build();
		}
		
		[AssignMe] [SerializeField]
		private LocalizedTextMesh text;
		
		[AssignMe] [SerializeField]
		private HoverButton hoverButton;
		
		private SubassemblyMeta _meta;
		public SubassemblyMeta subassemblyMeta {
			get => _meta;
			set
			{
				_meta = value;
				text.SetLocalizationKeyAndParams(text.LocalizationKey, _meta.title);
			}
		}
		
		// As SubassemblyCards are pooled, they have to be initialized once (the events must be linked once).
		public void Initialize()
		{
			hoverButton.OnClickEnd += onClicked;
		}
		
		public void onClicked()
		{
			Instances.Hotbar.AddItem(new SubassemblyHotbarItemData(subassemblyMeta.folder));
		}
		
		// Stuff for the searcher:
		
		public IReadOnlyList<string> NonLocalizedTags
		{
			get
			{
				var list = new List<string>();
				list.Add(subassemblyMeta.title);
				return list;
			}
		}
		
		public IReadOnlyList<string> LocalizedTags => null;
		public IReadOnlyList<string> CustomLocalizedTagCollectionKeys => null;
		public IReadOnlyList<string> NonLocalizedMatchFullTags => null;
		public IReadOnlyList<GameObject> DependentObjects => null;
	}
}
