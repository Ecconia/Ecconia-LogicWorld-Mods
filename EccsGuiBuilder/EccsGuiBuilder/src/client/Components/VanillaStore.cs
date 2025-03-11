using System;
using System.Linq;
using EccsGuiBuilder.Client.Layouts.Controller;
using EccsGuiBuilder.Client.Layouts.Elements;
using EccsLogicWorldAPI.Client.UnityHelper;
using EccsLogicWorldAPI.Shared;
using EccsLogicWorldAPI.Shared.AccessHelper;
using JimmysUnityUtilities;
using LogicLocalization;
using LogicUI.MenuTypes.ConfigurableMenus;
using LogicUI.Palettes;
using LogicWorld.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Types = EccsLogicWorldAPI.Shared.AccessHelper.Types;

namespace EccsGuiBuilder.Client.Components
{
	public static class VanillaStore
	{
		//A root object, containing the whole set of stock components. It merely serves as a wrapper to have the object-tree less cluttered.
		private static GameObject root;
		
		private static GameObject stockWindowCanvas;
		public static GameObject genWindowCanvas => stockWindowCanvas.clone();
		private static GameObject stockCanvas;
		public static GameObject genCanvas => stockCanvas.clone();
		
		private static GameObject stockSlider;
		public static GameObject genSlider => stockSlider.clone();
		private static GameObject stockToggle;
		public static GameObject genToggle => stockToggle.clone();
		private static GameObject stockButton;
		public static GameObject genButton => stockButton.clone();
		private static GameObject stockDivider;
		public static GameObject genDivider => stockDivider.clone();
		private static GameObject stockKeyHighlightBox;
		public static GameObject genKeyHighlightBox => stockKeyHighlightBox.clone();
		private static GameObject stockColorPicker;
		public static GameObject genColorPicker => stockColorPicker.clone();
		private static GameObject stockTextLine;
		public static GameObject genTextLine => stockTextLine.clone();
		private static GameObject stockTextArea;
		public static GameObject genTextArea => stockTextArea.clone();
		private static GameObject stockHelpCircle;
		public static GameObject genHelpCircle => stockHelpCircle.clone();
		private static GameObject stockInnerBox;
		public static GameObject genInnerBox => stockInnerBox.clone();
		
		private static GameObject stockSearchBar;
		public static GameObject genSearchBar => stockSearchBar.clone();
		private static GameObject stockScrollableVertical;
		public static GameObject genScrollableVertical => stockScrollableVertical.clone();
		
		private static Material guiFontMaterial;
		public static Material getGuiFontMaterial => guiFontMaterial;
		
		public static GameObject getWindowCanvas(string name, string titleKey = null, bool resizeX = true, bool resizeY = true, int resizeMinX = 400, int resizeMinY = 400)
		{
			var newWindowCanvas = Object.Instantiate(stockWindowCanvas);
			newWindowCanvas.SetActive(false);
			newWindowCanvas.name = name;
			var utilities = newWindowCanvas.GetComponent<ConfigurableMenuUtility>();
			if(titleKey == null)
			{
				utilities.TitleLocalizor.SetTextEmpty();
			}
			else
			{
				utilities.TitleLocalizor.SetLocalizationKey(titleKey);
			}
			
			var menu = newWindowCanvas.GetComponent<ConfigurableMenu>();
			Fields.getPrivate(menu, "IsResizableX").SetValue(menu, resizeX);
			Fields.getPrivate(menu, "IsResizableY").SetValue(menu, resizeY);
			Fields.getPrivate(menu, "MinWidth").SetValue(menu, resizeMinX);
			Fields.getPrivate(menu, "MinHeight").SetValue(menu, resizeMinY);
			if(!resizeX || !resizeY)
			{
				var fitter = GameObjectQuery.queryGameObject(newWindowCanvas, "GuiBuilder:WindowFrame").AddComponent<ContentSizeFitter>();
				if(!resizeX)
				{
					fitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
				}
				if(!resizeY)
				{
					fitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
				}
			}
			return newWindowCanvas;
		}
		
		public static void _addInternal(GameObject obj)
		{
			obj.setParent(root);
		}
		
		public static void init()
		{
			if(root != null)
			{
				return;
			}
			root = new GameObject("GuiBuilder:Container");
			root.SetActive(false); //This stays inactive forever. It has no position or setup, its just a wrapper to keep things sorted.
			
			//Grab interesting windows:
			/*
			 * The AND gate window provides:
			 * - Window frame
			 * - Slider
			 * - Slider text
			 */
			var editAndGateMenu = GameObjectQuery.queryGameObject("Edit AND Gate Menu");
			NullChecker.check(editAndGateMenu, "Could not find edit AND window");
			takeApartAndGateWindow(editAndGateMenu);
			/*
			 * The Key window provides:
			 * - Separator line
			 * - Button
			 * - Toggle
			 * - Toggle text
			 * - Color picker (incl: Popup + Color picker with colors)
			 * - "Key display area"
			 */
			var editKeyMenuContent = GameObjectQuery.queryGameObject("Edit Key Menu", "Menu", "Menu Content");
			NullChecker.check(editKeyMenuContent, "Could not find edit Key window content");
			takeApartKeyWindow(editKeyMenuContent);
			var editDisplayMainContent = GameObjectQuery.queryGameObject("Edit Display Menu", "Menu", "Menu Content", "Choose Display Configuration", "Scroll View", "Background");
			NullChecker.check(editKeyMenuContent, "Could not find edit Display window main content");
			{
				stockInnerBox = editDisplayMainContent.cloneWithParent(root);
				stockInnerBox.name = "Box";
				while(stockInnerBox.transform.childCount != 0)
				{
					Object.DestroyImmediate(stockInnerBox.getChild(0));
				}
			}
			
			var editLabelTextArea = GameObjectQuery.queryGameObject("Edit Label Menu", "Menu", "Menu Content", "Label Data Editor", "Left Side", "InputField scrollable");
			NullChecker.check(editAndGateMenu, "Could not find text field of Label Edit Window.");
			stockTextArea = editLabelTextArea.cloneWithParent(root);
			//Fix text color:
			var text = GameObjectQuery.queryGameObject(stockTextArea, "Scrolly Input Field", "Text Area", "Text");
			NullChecker.check(text, "Could not find TMP text inside of label editor text area.");
			var palette = text.GetComponent<PaletteGraphic>();
			NullChecker.check(palette, "TMP text inside of label editor text area has no Palette");
			palette.SetPaletteColor(PaletteColor.InputFieldText);
			//Fix placeholder color:
			text = GameObjectQuery.queryGameObject(stockTextArea, "Scrolly Input Field", "Text Area", "Placeholder");
			NullChecker.check(text, "Could not find TMP placeholder inside of label editor text area.");
			palette = text.GetComponent<PaletteGraphic>();
			NullChecker.check(palette, "TMP placeholder inside of label editor text area has no Palette");
			palette.SetPaletteColor(PaletteColor.InputFieldText);
			//Fix selection color:
			text = GameObjectQuery.queryGameObject(stockTextArea, "Scrolly Input Field"); //No need for null check, as accessed above.
			var palette2 = text.GetComponent<PaletteInputFieldSelection>();
			NullChecker.check(palette, "TMP placeholder inside of label editor text area has no Palette");
			palette2.SetPaletteOpacity(165);
			//Text-Color has to be injected on the spot.
			
			//Grab GUI font material:
			var textLabel = GameObjectQuery.queryGameObject(
				"Edit Label Menu",
				"Menu", "Menu Content",
				"Label Data Editor",
				"Left Side",
				"InputField scrollable",
				"Scrolly Input Field",
				"Text Area",
				"Text"
			);
			NullChecker.check(textLabel, "Could not find EditLabelMenu or the Text-Field inside");
			var tmp = textLabel.GetComponent<TextMeshProUGUI>();
			NullChecker.check(tmp, "Could not find TMP_UGUI inside of EditLabelMenu");
			guiFontMaterial = tmp.fontMaterial;
			
			//Get help entry:
			var helpCircle = GameObjectQuery.queryGameObject("Settings Menu", "Contents", "Right Side", "Profile Manager", "margins", "info");
			NullChecker.check(helpCircle, "Could not find help/info circle in game settings");
			stockHelpCircle = helpCircle.cloneWithParent(root);
			
			//Search bar from component selection menu:
			var searchBox = GameObjectQuery.queryGameObject("Selection Menu", "Menu", "Menu Content", "Search Box");
			NullChecker.check(searchBox, "Could not find search box in the component selection window");
			stockSearchBar = searchBox.cloneWithParent(root);
			
			//Scroll area/bar from Edit Display:
			var scrollableVertical = GameObjectQuery.queryGameObject("Edit Display Menu", "Menu", "Menu Content", "Choose Display Configuration", "Scroll View");
			NullChecker.check(searchBox, "Could not find vertical scrollarea in the edit display window");
			stockScrollableVertical = scrollableVertical.cloneWithParent(root);
			var scrollableVerticalContent = GameObjectQuery.queryGameObject(stockScrollableVertical, "Background", "Viewport", "Content");
			scrollableVerticalContent.transform.DestroyAllChildrenImmediate();
			//This part is kind of ugly, as there is a ContentSizeFitter Component and whatnot on this scroll content. The modders should sort this out, thus remove everything:
			foreach (var component in scrollableVerticalContent.GetComponents<Component>().Reverse())
			{
				//Do not remove the RectTransform, else Unity gets mad.
				if (!(component is RectTransform)) {
					Object.DestroyImmediate(component);
				}
			}
		}
		
		private static void takeApartAndGateWindow(GameObject editAndGateMenu)
		{
			stockWindowCanvas = Object.Instantiate(editAndGateMenu);
			stockWindowCanvas.name = "GuiBuilder:WindowCanvas";
			stockWindowCanvas.SetActive(false);
			stockWindowCanvas.setParent(root);
			//Cleanup frame:
			Object.DestroyImmediate(stockWindowCanvas.GetComponent<EditAndGateMenu>());
			
			//Create empty canvas:
			stockCanvas = stockWindowCanvas.cloneWithParent(root);
			//Remove like everything from the gameObject:
			//TODO: Improve code to be more generic...
			Object.DestroyImmediate(stockCanvas.getChild(0));
			Object.DestroyImmediate(stockCanvas.getChild(0));
			Object.DestroyImmediate(stockCanvas.getChild(0));
			Object.DestroyImmediate(stockCanvas.GetComponent<ConfigurableMenuUtility>());
			Object.DestroyImmediate(stockCanvas.GetComponent<ConfigurableMenu>());
			var t = Types.getType(typeof(StandardMenuBackground).Assembly, "LogicWorld.UI.AutomaticCloseButtonBehavior");
			Object.DestroyImmediate(stockCanvas.GetComponent(t));
			Object.DestroyImmediate(stockCanvas.GetComponent<StandardMenuBackground>());
			
			//Remove content:
			var content = GameObjectQuery.queryGameObject(stockWindowCanvas, "Menu", "Menu Content");
			content.name = "GuiBuilder:WindowContent";
			NullChecker.check(content, "Could not find Content in edit AND window");
			if(content.transform.childCount != 2)
			{
				throw new Exception("GuiBuilder: Something change inside of the AND gates content, not 2 children.");
			}
			
			//TBI: What to do with this label, it is obviously looking bad in LW. Has to be changed and wrapped.
			var tinyLabel = content.getChild(0);
			tinyLabel.setParent(root); //Unbind the label from the AND gate window, as the window frame has to be empty.
			
			stockSlider = content.getChild(0); //First child got removed, hence this is first again.
			stockSlider.GetComponent<RectTransform>().pivot = Vector2.up;
			stockSlider.setParent(root); //Unbind the slider from the AND gate window, as the window frame has to be empty.
			
			//Finally, fix the window frame:
			
			/*
			 * Before:
			 * WindowFrame
			 * - Content (GuiBuilder:WindowContent)
			 * - Title Bar
			 *   - Dividing Line
			 *   - Title Bar Contents
			 *     - Title
			 *     - Show Menu Settings
			 *     - Close
			 * - Outline
			 * - Resizing Friends
			 */
			
			/*
			 * After:
			 * GuiBuilder:WindowFrame
			 * - GuiBuilder:WindowContent
			 * - GuiBuilder:WindowDivider
			 * - GuiBuilder:TitleBar
			 *   - Title
			 *   - Title Bar Contents
			 *     - Show Menu Settings
			 *     - Close
			 * - GuiBuilder:WindowOutline
			 * - GuiBuilder:ResizingArrows
			 */
			
			var stockWindowFrame = GameObjectQuery.queryGameObject(stockWindowCanvas, "Menu");
			stockWindowFrame.name = "GuiBuilder:WindowFrame";
			stockWindowFrame.AddComponent<WindowLayout>();
			var stockTitleBar = stockWindowFrame.getChild(1);
			stockTitleBar.name = "GuiBuilder:TitleBar";
			stockTitleBar.AddComponent<TitleBarLayout>(); //Custom layout.
			//Move divider where it should be:
			var divider = stockTitleBar.getChild(0);
			divider.name = "GuiBuilder:WindowDivider";
			divider.setParent(stockWindowFrame);
			divider.transform.SetSiblingIndex(1); //Make second entry
			divider.AddComponent<DividerLayout>();
			//Fix title bar:
			var titleBarContents = stockTitleBar.getChild(0);
			var title = titleBarContents.getChild(0);
			title.setParent(stockTitleBar);
			title.transform.SetAsFirstSibling();
			var titleTMP = title.GetComponent<TextMeshProUGUI>();
			titleTMP.enableAutoSizing = false;
			titleTMP.fontSize = 40.35f;
			titleTMP.textWrappingMode = TextWrappingModes.PreserveWhitespaceNoWrap;
			var titleRect = title.GetComponent<RectTransform>();
			titleRect.pivot = new Vector2(0, 1);
			titleRect.anchoredPosition = new Vector2(10, 2);
			//Fix title bar content:
			titleBarContents.name = "GuiBuilder:TitleBarContent";
			var titleBarContentsRect = titleBarContents.GetComponent<RectTransform>();
			titleBarContentsRect.sizeDelta = Vector2.zero;
			titleBarContentsRect.anchoredPosition = new Vector2(-10, 0);
			var titleBarContentsLayout = titleBarContents.AddComponent<HorizontalLayoutGroup>();
			titleBarContentsLayout.childAlignment = TextAnchor.MiddleRight;
			titleBarContentsLayout.spacing = 10;
			titleBarContentsLayout.childForceExpandHeight = false;
			titleBarContentsLayout.childForceExpandWidth = false;
			titleBarContents.getChild(0).AddComponent<TitleBarButtonLayout>();
			titleBarContents.getChild(1).AddComponent<TitleBarButtonLayout>();
			//Fix outline & resizing arrows:
			var outline = stockWindowFrame.getChild(3);
			outline.name = "GuiBuilder:WindowOutline";
			outline.AddComponent<IgnoreLayout>();
			var resizingArrows = stockWindowFrame.getChild(4);
			resizingArrows.name = "GuiBuilder:ResizingArrows";
			resizingArrows.AddComponent<IgnoreLayout>();
		}
		
		private static void takeApartKeyWindow(GameObject editKeyMenuContent)
		{
			var rawDivider = GameObjectQuery.queryGameObject(editKeyMenuContent, "dividing line");
			NullChecker.check(rawDivider, "Edit Key window has no dividing line");
			stockDivider = rawDivider.cloneWithParent(root);
			stockDivider.GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
			
			var top = GameObjectQuery.queryGameObject(editKeyMenuContent, "Top");
			NullChecker.check(top, "Edit Key window has no top section");
			var bottom = GameObjectQuery.queryGameObject(editKeyMenuContent, "Bottom");
			NullChecker.check(bottom, "Edit Key window has no bottom section");
			
			var rawToggleSwitchText = GameObjectQuery.queryGameObject(bottom, "Localized Text");
			NullChecker.check(rawToggleSwitchText, "EditKeyWindow has no toggle text");
			// var textLineWrapper = new GameObject("GuiBuilder:TextLine");
			// textLineWrapper.setParent(root);
			stockTextLine = rawToggleSwitchText.cloneWithParent(root);
			stockTextLine.name = "GuiBuilder:Text";
			stockTextLine.GetComponent<LocalizedTextMesh>().SetTextEmpty();
			var tmp = stockTextLine.GetComponent<TextMeshProUGUI>();
			tmp.enableAutoSizing = false;
			tmp.fontSize = 50;
			tmp.textWrappingMode = TextWrappingModes.PreserveWhitespaceNoWrap;
			stockTextLine.AddComponent<TMPFixer>();
			
			var rawToggleSwitch = GameObjectQuery.queryGameObject(bottom, "Toggle Switch");
			NullChecker.check(rawToggleSwitch, "EditKeyWindow has no toggle");
			fixToggle(rawToggleSwitch.cloneWithParent(root));
			
			//Setup stock button:
			var rawButton = GameObjectQuery.queryGameObject(top, "Edit Keybinding Button");
			NullChecker.check(rawButton, "Edit Key window has no edit-keybinding button");
			stockButton = rawButton.cloneWithParent(root);
			stockButton.name = "Button";
			stockButton.GetComponentInChildren<LocalizedTextMesh>().SetLocalizationKey("Unset.Localization.Key.Set.Me.Please");
			var textMeshPro = stockButton.GetComponentInChildren<TextMeshProUGUI>();
			textMeshPro.enableAutoSizing = false;
			textMeshPro.fontSize = 50;
			
			//Key highlight box:
			var rawKeyHighlightBox = GameObjectQuery.queryGameObject(top, "Background Graphic");
			NullChecker.check(rawKeyHighlightBox, "EditKeyWindow has no key-highlight-box");
			stockKeyHighlightBox = rawKeyHighlightBox.cloneWithParent(root);
			stockKeyHighlightBox.name = "KeyHighlightBox";
			Object.DestroyImmediate(stockKeyHighlightBox.GetComponent<AspectRatioFitter>());
			stockKeyHighlightBox.GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
			
			//Color picker module:
			var rawColorPicker = GameObjectQuery.queryGameObject(top, "Foreground Picker");
			NullChecker.check(rawKeyHighlightBox, "EditKeyWindow has no foreground color picker");
			stockColorPicker = rawColorPicker.cloneWithParent(root);
			stockColorPicker.name = "ColorPicker";
		}
		
		private static void fixToggle(GameObject rawToggle)
		{
			//TBI: Inject these values later on, but these are the default for now:
			const float indicatorMaxWidth = 60f;
			const float lineHeight = 50f;
			
			var newToggle = new GameObject("Toggle Switch");
			newToggle.AddComponent<RectTransform>();
			
			var toggle = rawToggle.cloneWithParent(newToggle);
			toggle.name = "Toggle";
			Object.DestroyImmediate(toggle.GetComponent<AspectRatioFitter>()); //The size for toggles is fixed with the layout system. Hence do not need this anymore.
			var rect = toggle.GetComponent<RectTransform>();
			rect.anchorMin = new Vector2(0, 0);
			rect.sizeDelta = new Vector2(-indicatorMaxWidth, 0); //Subtract the indicator.
			
			var onIndicator = toggle.getChild(1);
			rect = onIndicator.GetComponent<RectTransform>();
			rect.pivot = new Vector2(1, 0.5f);
			rect.sizeDelta = new Vector2(indicatorMaxWidth, 0);
			onIndicator.setParent(newToggle); //Extract ON-Indicator to new wrapper.
			
			stockToggle = newToggle;
			var layout = stockToggle.AddComponent<DefinedSizeLayout>();
			layout.size = new Vector2(lineHeight * 1.7f + indicatorMaxWidth, lineHeight);
		}
	}
}
