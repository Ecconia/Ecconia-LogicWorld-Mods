using System;
using System.Text;
using JimmysUnityUtilities;
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
		private const int fileCustomDataLength = 256 + 8;
		private static readonly char[] mapping = new char[256];
		
		//TODO: Add control characters, such as space and newline.
		static Ram256b8()
		{
			var index = 1; //Skip the first character.
			for(var i = 0; i < 26; i++)
			{
				mapping[index++] = (char) ('A' + i);
			}
			for(var i = 0; i < 26; i++)
			{
				mapping[index++] = (char) ('a' + i);
			}
			char[] otherCharacters =
			{
				'?', '!', '.', ',', ':', '\'', '"',
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
				'+', '-', '×', '/', '=', '(', ')', '→', '←', '↓', '↑',
			};
			for(var i = 0; i < otherCharacters.Length; i++)
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
						Position = new Vector3(0f, Ram256b8PrefabGenerator.height, 0f),
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
				setup(DateTime.Now, 2);
			}
		}
		
		private void setup(DateTime started, byte isFirstFrame)
		{
			var playerName = Helper.getPlayerName();
			if(playerName == null)
			{
				//TODO: Instead of hooking to world initialization, hook to something that triggers once world join is finished - or better loop until world has finished loading.
				if(isFirstFrame == 1)
				{
					//Likely, that the first frame happened while joining. So lets wait for the next frame, where the game is closer to the actual joining (when the player name gets sent by the server).
					started = DateTime.Now;
				}
				if(isFirstFrame != 0)
				{
					isFirstFrame--;
				}
				if((DateTime.Now - started).Seconds > 10)
				{
					ModClass.logger.Error("Not able to request memory data from the server, as it never sent the player name.");
					return; //Just stop now.
				}
				//Try again next frame (keep on until data is sent by the server, but no longer than 4 seconds):
				CoroutineUtility.RunAfterOneFrame(() => setup(started, isFirstFrame));
				return;
			}
			//Send the name encoded as UTF8 to the server:
			var bytes = Encoding.UTF8.GetBytes(playerName);
			var isCritical = (bytes.Length + 1) == fileCustomDataLength;
			var prefix = isCritical ? 2 : 1;
			var newBytes = new byte[bytes.Length + prefix];
			Buffer.BlockCopy(bytes, 0, newBytes, prefix, bytes.Length);
			if(isCritical)
			{
				newBytes[0] = 1;
			}
			BuildRequestManager.SendBuildRequestWithoutAddingToUndoStack(new BuildRequest_UpdateComponentCustomData(Address, newBytes));
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
				for(var i = 0; i < 256; i++)
				{
					setValue(i, data[i]);
				}
				isInitialized = true;
			}
			else if(data.Length == 2)
			{
				//In the case that the memory component failed to initialize,
				// at least update the slots that are received here, these might be overwritten later.
				// This should never be an issue (since single-threading).
				setValue(data[0], data[1]); //Update data.
			}
		}
		
		private void setValue(int index, int value)
		{
			var label = Decorations[0].DecorationObject.GetComponent<TextMeshPro>();
			var text = label.text.ToCharArray();
			var offset = index * 11 + 5;
			//0123456789
			//123: vvv M
			var v = value.ToString();
			var iv = 0;
			text[offset++] = v.Length < 3 ? ' ' : v[iv++];
			text[offset++] = v.Length < 2 ? ' ' : v[iv++];
			text[offset] = v[iv];
			offset += 2;
			var mapped = mapping[value];
			text[offset] = value == 0 ? ' ' : mapped == 0 ? '…' : mapped;
			label.text = new string(text);
		}
		
		protected override IDecoration[] GenerateDecorations(Transform parentToCreateDecorationsUnder)
		{
			var alignment = Quaternion.AngleAxis(-90, Vector3.up) * Quaternion.AngleAxis(-90, Vector3.forward);
			
			var (go, label) = Helper.textObjectMono("Ram256b8: TextDecoration");
			var rect = go.GetComponent<RectTransform>();
			rect.sizeDelta = new Vector2(0, 0.5625f);
			rect.pivot = new Vector2(0, -1);
			label.fontSize = 5.625f;
			label.lineSpacing = -36.2f;
			label.autoSizeTextContainer = false;
			label.horizontalAlignment = HorizontalAlignmentOptions.Left;
			label.verticalAlignment = VerticalAlignmentOptions.Top;
			label.textWrappingMode = TextWrappingModes.PreserveWhitespaceNoWrap;
			var sb = new StringBuilder(256 * 11);
			for(var i = 255; i >= 0; i--)
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
			
			go.transform.SetParent(parentToCreateDecorationsUnder);
			
			return new IDecoration[]
			{
				new Decoration()
				{
					LocalPosition = new Vector3(0.155f, (Ram256b8PrefabGenerator.height) * 0.3f - 0.15f, 254.5f * 0.5625f),
					LocalRotation = alignment,
					DecorationObject = go,
					AutoSetupColliders = false,
					IncludeInModels = false,
					ShouldBeOutlined = false,
				},
			};
		}
	}
}
