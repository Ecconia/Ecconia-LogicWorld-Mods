using System;
using System.Reflection;
using LogicUI;
using LogicWorld.Players;
using LogicWorld.References;
using TMPro;
using UnityEngine;

namespace EcconiaCPUServerComponents.Client
{
	public static class Helper
	{
		static Helper()
		{
			nametagField = initializeNametagReflection();
		}
		
		//### Decoration GameObject helper: ##############
		
		public static (GameObject, TextMeshPro) textObjectMono(string name)
		{
			var gameObject = new GameObject(name);
			gameObject.SetActive(false); //Set it to inactive, so that TMP won't corrupt the default material
			var textRenderer = gameObject.AddComponent<TextMeshPro>(); //Add TMP to the GameObject
			textRenderer.fontSharedMaterial = Materials.NotoSansMono_WorldSpace; //Set its material to a copy of what LW uses
			textRenderer.font = Fonts.NotoMono; //Also set the font to a copy of what LW uses
			gameObject.SetActive(true); //Set to active, as the critical part is over
			return (gameObject, textRenderer);
		}
		
		//### Reflection access helper: ############
		
		private static readonly FieldInfo nametagField;
		
		private static FieldInfo initializeNametagReflection()
		{
			//Reflection:
			var nametagFieldInner = typeof(PlayerModelNametag).GetField("Nametag", BindingFlags.NonPublic | BindingFlags.Instance);
			if(nametagFieldInner == null)
			{
				throw new Exception("Did not find field 'Nametag' in 'PlayerModelNametag'");
			}
			return nametagFieldInner;
		}
		
		public static string getPlayerName()
		{
			var playerModel = PlayerModelsManager.PlayerModelSelf;
			if(playerModel == null)
			{
				return null;
			}
			var tmp = (TextMeshPro) nametagField.GetValue(playerModel.Appearance.Nametag);
			if(tmp == null)
			{
				throw new Exception("TextMeshPro field in the NameTag thing not set yet :/");
			}
			return tmp.text;
		}
	}
}
