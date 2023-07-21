using EccsLogicWorldAPI.Shared;
using LogicUI.HoverTags;
using LogicUI.Palettes;
using UnityEngine;

namespace EccsGuiBuilder.Client.Wrappers.Specialized
{
	public class HelpWrapper : Wrapper<HelpWrapper>
	{
		private readonly HoverTagArea_Localized hoverText;
		private readonly PaletteGraphic backgroundColor;
		
		public HelpWrapper(GameObject gameObject) : base(gameObject)
		{
			hoverText = gameObject.GetComponentInChildren<HoverTagArea_Localized>();
			NullChecker.check(hoverText, "Could not find HoverTagArea_Localized inside of GameObject");
			backgroundColor = gameObject.GetComponent<PaletteGraphic>();
			NullChecker.check(hoverText, "Could not find PaletteGraphic inside of GameObject");
		}
		
		public HelpWrapper setLocalizationKey(string text)
		{
			hoverText.LocalizationKey = text;
			return this;
		}
		
		public HelpWrapper setColor(PaletteColor color)
		{
			backgroundColor.SetPaletteColor(color);
			return this;
		}
	}
}
