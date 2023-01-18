using System.Collections.Generic;
using System.Text;
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
				'?', '!', '.', ',', ':', '\'', '"',
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
				for(int i = 0; i < 256; i++)
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
			TextMeshPro label = GetDecorations()[0].DecorationObject.GetComponent<TextMeshPro>();
			var text = label.text.ToCharArray();
			int offset = index * 11 + 5;
			//0123456789
			//123: vvv M
			string v = value.ToString();
			int iv = 0;
			text[offset++] = v.Length < 3 ? ' ' : v[iv++];
			text[offset++] = v.Length < 2 ? ' ' : v[iv++];
			text[offset] = v[iv];
			offset += 2;
			char mapped = mapping[value];
			text[offset] = value == 0 ? ' ' : mapped == 0 ? '…' : mapped;
			label.text = new string(text);
		}

		protected override IList<IDecoration> GenerateDecorations()
		{
			Quaternion alignment = Quaternion.AngleAxis(-90, Vector3.up) * Quaternion.AngleAxis(-90, Vector3.forward);

			(GameObject go, TextMeshPro label) = Helper.textObjectMono("Ram256b8: TextDecoration");
			RectTransform rect = go.GetComponent<RectTransform>();
			rect.sizeDelta = new Vector2(0, 0.5625f);
			rect.pivot = new Vector2(0, -1);
			label.fontSize = 5.625f;
			label.lineSpacing = -36.2f;
			label.autoSizeTextContainer = false;
			label.horizontalAlignment = HorizontalAlignmentOptions.Left;
			label.verticalAlignment = VerticalAlignmentOptions.Top;
			label.enableWordWrapping = false;
			StringBuilder sb = new StringBuilder(256 * 11);
			for(int i = 255; i >= 0; i--)
			{
				if(i < 10)
				{
					sb.Append(' ');
				}
				if(i < 100)
				{
					sb.Append(' ');
				}
				sb.Append(i).Append(": ???  \n");
			}
			label.text = sb.ToString();
			label.color = new Color(0f, 0.8f, 0.6f);

			return new List<IDecoration>()
			{
				new Decoration()
				{
					LocalPosition = new Vector3(0.155f, (Ram256b8Prefab.height) * 0.3f - 0.15f, 254.5f * 0.5625f),
					LocalRotation = alignment,
					DecorationObject = go,
					AutoSetupColliders = false,
					IncludeInModels = false,
				}
			};
		}
	}
}
