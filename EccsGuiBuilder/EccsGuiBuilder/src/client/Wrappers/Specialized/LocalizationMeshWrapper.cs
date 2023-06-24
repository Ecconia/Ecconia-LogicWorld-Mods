using System;
using EccsLogicWorldAPI.Shared;
using LogicLocalization;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EccsGuiBuilder.Client.Wrappers.Specialized
{
	public class LocalizationMeshWrapper : Wrapper<LocalizationMeshWrapper>
	{
		private readonly TextMeshProUGUI text;
		private readonly LocalizedTextMesh localizedTextMesh;
		
		public LocalizationMeshWrapper(GameObject gameObject) : base(gameObject)
		{
			text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
			NullChecker.check(text, "Could not find TextMeshProUGUI inside of GameObject");
			localizedTextMesh = gameObject.GetComponentInChildren<LocalizedTextMesh>();
			NullChecker.check(localizedTextMesh, "Could not find LocalizedTextMesh inside of GameObject");
		}
		
		public LocalizationMeshWrapper setLocalizationKey(string text)
		{
			localizedTextMesh.SetLocalizationKey(text);
			return this;
		}
		
		public LocalizationMeshWrapper setLocalizationKeyAndParams(string text, params object[] localizationParams)
		{
			localizedTextMesh.SetLocalizationKeyAndParams(text, localizationParams);
			return this;
		}
		
		public LocalizationMeshWrapper removeLocalization()
		{
			Object.DestroyImmediate(localizedTextMesh);
			return this;
		}
		
		public LocalizationMeshWrapper setFontSize(float fontSize)
		{
			text.enableAutoSizing = false; //Precaution
			text.fontSize = fontSize;
			return this;
		}
		
		public LocalizationMeshWrapper configureTMP(Action<TextMeshProUGUI> action)
		{
			action(text);
			return this;
		}
	}
}
