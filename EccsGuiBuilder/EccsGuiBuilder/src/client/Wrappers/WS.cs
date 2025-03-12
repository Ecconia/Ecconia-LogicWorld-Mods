using EccsGuiBuilder.Client.Components;
using EccsGuiBuilder.Client.Wrappers.RootWrappers;
using EccsGuiBuilder.Client.Wrappers.Specialized;
using UnityEngine;

namespace EccsGuiBuilder.Client.Wrappers
{
	//Wrapper Store
	public static class WS
	{
		public static CanvasWrapper canvas(string name) => new CanvasWrapper(VanillaStore.genCanvas, name);
		public static WindowWrapper window(string name) => new WindowWrapper(VanillaStore.genWindowCanvas, name);
		public static LocalizationMeshWrapper button => new LocalizationMeshWrapper(VanillaStore.genButton);
		public static SimpleWrapper colorPicker => new SimpleWrapper(VanillaStore.genColorPicker);
		public static SimpleWrapper keyHighlighter => new SimpleWrapper(VanillaStore.genKeyHighlightBox);
		public static LocalizationMeshWrapper textLine => new LocalizationMeshWrapper(VanillaStore.genTextLine);
		public static TextFieldWrapper inputField => new TextFieldWrapper(CustomStore.genInputField);
		public static TextFieldWrapper inputArea => new TextFieldWrapper(VanillaStore.genTextArea);
		public static SliderWrapper slider => new SliderWrapper(VanillaStore.genSlider);
		public static SimpleWrapper toggle => new SimpleWrapper(VanillaStore.genToggle);
		public static HelpWrapper help => new HelpWrapper(VanillaStore.genHelpCircle);
		
		public static SimpleWrapper searchBox => new SimpleWrapper(VanillaStore.genSearchBar);
		public static ScrollareaWrapper scrollableVertical => new ScrollareaWrapper(VanillaStore.genScrollableVertical);
		
		public static SimpleWrapper wrap(GameObject obj)
		{
			return new SimpleWrapper(obj);
		}
		
		public static SimpleWrapper empty(string name)
		{
			var gameObject = new GameObject(name);
			gameObject.AddComponent<RectTransform>();
			return new SimpleWrapper(gameObject);
		}
	}
}
