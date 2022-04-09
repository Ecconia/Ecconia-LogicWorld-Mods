using System.Collections.Generic;
using LogicAPI.Data.BuildingRequests;
using LogicWorld.BuildingManagement;
using LogicWorld.Interfaces;
using LogicWorld.Interfaces.Building;
using LogicWorld.Rendering.Chunks;
using LogicWorld.Rendering.Components;
using TMPro;
using UnityEngine;

namespace EcconiaCPUServerComponents.Client
{
	public class Ram256b8 : ComponentClientCode
	{
		private static readonly char[] mapping = new char[256];

		//TODO: Add control characters, such as space and newline.
		static Ram256b8()
		{
			int index = 1; //Skip the first character.
			for(int i = 0; i < 26; i++)
			{
				mapping[index++] = (char) ('A' + i);
			}
			for(int i = 0; i < 26; i++)
			{
				mapping[index++] = (char) ('a' + i);
			}
			char[] otherCharacters =
			{
				'?', '.', ',', ':', '\'', '"',
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
				'+', '-', '×', '/', '=', '(', ')', '→', '←', '↓', '↑',
			};
			for(int i = 0; i < otherCharacters.Length; i++)
			{
				mapping[index++] = otherCharacters[i];
			}
		}
		
		private bool isInitialized;
		
		protected override ChildPlacementInfo GenerateChildPlacementInfo()
		{
			return new ChildPlacementInfo()
			{
				Points = new FixedPlacingPoint[]
				{
					new FixedPlacingPoint()
					{
						Position = new Vector3(0f, Ram256b8Prefab.height, 0f),
						Range = 100f,
					},
				},
			};
		}

		protected override void InitializeInWorld()
		{
			//Gets apparently called whenever this component is placed.
			// So only ask for state update, once the component gets created.
			// No need to update 
			if(!isInitialized)
			{
				// LConsole.WriteLine("Requesting broadcast.");
				BuildRequestManager.SendBuildRequestWithoutAddingToUndoStack(new BuildRequest_UpdateComponentCustomData(base.Address, new byte[]
				{
					0, //A single 0 from now on means "broadcast state".
				}), null);
			}
		}

		protected override void DeserializeData(byte[] data)
		{
			if(data == null || data.Length == 1)
			{
				return; //We do not care.
			}
			if(data.Length == 256)
			{
				//Full data update:
				// LConsole.WriteLine("Got full update.");
				for(int i = 0; i < GetDecorations().Count; i++)
				{
					setValue(i, data[i]);
				}
				isInitialized = true;
			}
			else if(data.Length == 2)
			{
				if(!isInitialized)
				{
					// LConsole.WriteLine("Ignoring data update.");
					return;
				}
				//Update packet!
				// LConsole.WriteLine("Got update: [" + data[0] + "] = " + data[1]);
				setValue(data[0], data[1]);
			}
		}

		private void setValue(int index, int value)
		{
			TextMeshPro label = GetDecorations()[index].DecorationObject.GetComponent<TextMeshPro>();
			string textIndex = index > 99 ? index.ToString() : index > 9 ? " " + index : "  " + index;
			string textValue = value > 99 ? value.ToString() : value > 9 ? " " + value : "  " + value;
			char mapped = mapping[value];
			string textMapped = value == 0 ? "" : mapped == 0 ? " …" : " " + mapped; 
			label.text = "<mspace=0.5em>" + textIndex + ": " + textValue + textMapped + "</mspace>";
		}

		protected override IList<IDecoration> GenerateDecorations()
		{
			IList<IDecoration> decorations = new List<IDecoration>(256);
			Quaternion alignment = Quaternion.AngleAxis(-90, Vector3.up) * Quaternion.AngleAxis(-90, Vector3.forward);
			for(int i = 0; i < 256; i++)
			{
				GameObject go = new GameObject();
				RectTransform rect = go.AddComponent<RectTransform>();
				rect.sizeDelta = new Vector2(0, 0.5625f);
				TextMeshPro label = go.AddComponent<TextMeshPro>();
				label.fontSize = 5.625f;
				label.autoSizeTextContainer = false;
				label.horizontalAlignment = HorizontalAlignmentOptions.Left;
				label.verticalAlignment = VerticalAlignmentOptions.Middle;
				label.enableWordWrapping = false;
				label.text = "<mspace=0.5em>" + i + ": ???</mspace>";
				label.color = new Color(0f, 0.8f, 0.6f);
				decorations.Add(new Decoration()
				{
					LocalPosition = new Vector3(0.155f, (Ram256b8Prefab.height) * 0.3f - 0.15f, (i + 0.5f) * 0.5625f),
					LocalRotation = alignment,
					DecorationObject = go,
					AutoSetupColliders = false,
					IncludeInModels = false,
				});
			}
			return decorations;
		}
	}
}
