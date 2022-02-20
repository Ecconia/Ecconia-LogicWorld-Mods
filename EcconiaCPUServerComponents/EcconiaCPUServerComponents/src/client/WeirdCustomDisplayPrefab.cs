//Needed for 'Exception':
using System;
//Needed for everything else :)
using LogicWorld.Interfaces;
using LogicWorld.References;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode;
using LogicWorld.SharedCode.Components;
//Needed for 'Vector3' (and maybe others):
using UnityEngine;

namespace Ecconia.CPUServerComponents.Client
{
	//I do not need to put this code here, I could put it in the SUCC file.
	// But then I think I would go crazy. So here it stays.
	public class WeirdCustomDisplayPrefab : PrefabVariantInfo
	{
		//MUST BE AN EVEN NUMBER, OR CODE AND MODEL BREAK! Actually it is used everywhere now, do not change.
		private const int DisplaySideLength = 32;

		public override string ComponentTextID => "EcconiaCPUServerComponents.WeirdCustomDisplay";

		public override PrefabVariantIdentifier GetDefaultComponentVariant()
		{
			// (32 'Selector' + 32 'Invert Selector') * 2 'X/Y' + 1 'Data'
			return new PrefabVariantIdentifier(DisplaySideLength * 4 + 1, 0);
		}

		public override ComponentVariant GenerateVariant(PrefabVariantIdentifier identifier)
		{
			if(identifier.InputCount != (DisplaySideLength * 4 + 1) || identifier.OutputCount != 0)
			{
				throw new Exception("Attempted to create Ecconias WeirdCustomDisplay with unexpected peg configuration. Loading old save? Wrong mod version?");
			}

			//Blocks:
			Block[] blocks = new Block[DisplaySideLength * DisplaySideLength + 1];
			{
				//Set Display faces:
				int index = 0;
				for(int y = 0; y < DisplaySideLength; y++)
				{
					for(int x = 0; x < DisplaySideLength; x++)
					{
						blocks[index++] = new Block
						{
							Scale = new Vector3(2f, 1f, 2f),
							Rotation = new Vector3(90f, 0f, 0f),
							Position = new Vector3(
								-.5f - x * 2f,
								1f + y * 2f,
								.25f
							),
							Mesh = Meshes.FlatQuad,
							RawColor = Colors.DisplayOff,
							ColliderData = new ColliderData
							{
								Type = ColliderType.None,
							},
						};
					}
				}
				//Currently debugging main block:
				blocks[index] = new Block()
				{
					Scale = new Vector3(DisplaySideLength * 2f, 1f, DisplaySideLength * 2f),
					Rotation = new Vector3(-90f, 0f, 0f),
					Position = new Vector3(.5f - DisplaySideLength, DisplaySideLength, .25f),
					Mesh = Meshes.BetterCube_OpenBottom,
				};
			}

			//Inputs:
			ComponentInput[] inputs = new ComponentInput[DisplaySideLength * 4 + 1];
			{
				float middleX = +.5f - DisplaySideLength;
				float middleY = DisplaySideLength;
				float halfSide = DisplaySideLength / 2f;

				int index = 0;
				//Invert X:
				for(int x = 0; x < DisplaySideLength; x += 2)
				{
					inputs[index++] = new ComponentInput
					{
						Length = 0.6f,
						Rotation = new Vector3(-90f, 0, 0),
						Position = new Vector3(
							middleX + (halfSide - x) * 2f - 1f - 0.5f,
							middleY,
							-.75f
						),
					};
					inputs[index++] = new ComponentInput
					{
						Length = 0.6f,
						Rotation = new Vector3(-90f, 0, 0),
						Position = new Vector3(
							middleX + (halfSide - x) * 2f - 1f - 1.5f,
							middleY,
							-.75f
						),
					};
				}
				//Data X:
				for(int x = 0; x < DisplaySideLength; x++)
				{
					inputs[index++] = new ComponentInput
					{
						Rotation = new Vector3(-90f, 0, 0),
						Position = new Vector3(
							middleX + (halfSide - x) * 2f - 1f,
							middleY,
							-.75f
						),
					};
				}
				//Invert Y:
				for(int y = 0; y < DisplaySideLength; y += 2)
				{
					inputs[index++] = new ComponentInput
					{
						Length = 0.6f,
						Rotation = new Vector3(-90f, 0, 0),
						Position = new Vector3(
							middleX,
							middleY - (halfSide - y) * 2f + 1f + 0.5f,
							-.75f
						),
					};
					inputs[index++] = new ComponentInput
					{
						Length = 0.6f,
						Rotation = new Vector3(-90f, 0, 0),
						Position = new Vector3(
							middleX,
							middleY - (halfSide - y) * 2f + 1f + 1.5f,
							-.75f
						),
					};
				}
				//Data Y:
				for(int y = 0; y < DisplaySideLength; y++)
				{
					inputs[index++] = new ComponentInput
					{
						Rotation = new Vector3(-90f, 0, 0),
						Position = new Vector3(
							middleX,
							middleY - (halfSide - y) * 2f + 1f,
							-.75f
						),
					};
				}
				//Data:
				inputs[index] = new ComponentInput
				{
					Length = 1.1f, //Default is: 0.8
					Rotation = new Vector3(-90f, 0, 0),
					Position = new Vector3(middleX, middleY, -.75f),
					//WirePointHeight = 0.9f, //Default
					//Bottomless = true, //Default
					//StartOn = false, //Default
				};
			}
			return new ComponentVariant
			{
				VariantPrefab = new Prefab
				{
					Blocks = blocks,
					Inputs = inputs,
				},
				VariantPlacingRules = new PlacingRules
				{
					PrimaryEdgePositions = new Vector2[]
					{
						new Vector2(0.5f, 0f),
					},
				},
			};
		}
	}
}
