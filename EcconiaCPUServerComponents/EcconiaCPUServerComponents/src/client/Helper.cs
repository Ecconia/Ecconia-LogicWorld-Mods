using LogicUI;
using LogicWorld.References;
using TMPro;
using UnityEngine;

namespace EcconiaCPUServerComponents.Client
{
	public static class Helper
	{
		public static (GameObject, TextMeshPro) textObjectMono(string name)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.SetActive(false); //Set it to inactive, so that TMP won't corrupt the default material
			TextMeshPro textRenderer = gameObject.AddComponent<TextMeshPro>(); //Add TMP to the GameObject
			textRenderer.fontSharedMaterial = Object.Instantiate(Materials.NotoSansMono_WorldSpace); //Set its material to a copy of what LW uses
			textRenderer.font = Object.Instantiate(Fonts.NotoMono); //Also set the font to a copy of what LW uses
			gameObject.SetActive(true); //Set to active, as the critical part is over
			return (gameObject, textRenderer);
		}
	}
}
