using System;
using JimmysUnityUtilities;
using LogicWorld.Interfaces;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using UnityEngine;

namespace EcconiaCPUServerComponents.Client
{
	public class Ram256b8Prefab : PrefabVariantInfo
	{
		//Units in squares:
		public static readonly float width = 20f;
		public static readonly float depth = 481.5f;
		public static readonly float height = 13f;

		private static readonly int dataWidth = 8;
		private static readonly int outputCount = dataWidth;
		private static readonly int inputCount = 9 + 9 + dataWidth;

		public override string ComponentTextID => "EcconiaCPUServerComponents.Ram256b8";

		public override PrefabVariantIdentifier GetDefaultComponentVariant()
		{
			return new PrefabVariantIdentifier(inputCount, outputCount);
		}

		public override ComponentVariant GenerateVariant(PrefabVariantIdentifier identifier)
		{
			if(identifier.InputCount != inputCount || identifier.OutputCount != outputCount)
			{
				throw new Exception("Attempted to create Ecconias Ram256b8 with unexpected peg configuration. Loading old save? Wrong mod version? Inputs: " + identifier.InputCount + " Outputs: " + identifier.OutputCount);
			}

			ComponentInput[] inputs = new ComponentInput[inputCount];
			int index = 0;
			//WRITE:
			for(int i = 0; i < 9; i++)
			{
				inputs[index++] = new ComponentInput()
				{
					Position = new Vector3(
						-width + 1.5f + 8f - i,
						7.5f,
						-.5f
					),
					Rotation = new Vector3(-90, 0, 0),
				};
			}

			//READ:
			for(int i = 0; i < 9; i++)
			{
				inputs[index++] = new ComponentInput()
				{
					Position = new Vector3(
						-width + 2f + 8f - i,
						6.5f,
						-.5f
					),
					Rotation = new Vector3(-90, 0, 0),
				};
			}

			//DATA:
			for(int i = 0; i < 8; i++)
			{
				inputs[index++] = new ComponentInput()
				{
					Position = new Vector3(
						0f - i,
						3.5f,
						-.5f
					),
					Rotation = new Vector3(-90, 0, 0),
				};
			}

			ComponentOutput[] outputs = new ComponentOutput[outputCount];
			for(int i = 0; i < dataWidth; i++)
			{
				outputs[i] = new ComponentOutput()
				{
					Position = new Vector3(
						0f - i,
						2.5f,
						-.5f
					),
					Rotation = new Vector3(-90, 0, 0),
				};
			}

			return new ComponentVariant()
			{
				VariantPrefab = new Prefab()
				{
					Blocks = new Block[]
					{
						new Block()
						{
							//Default block, nothing special - lets see.
							Scale = new Vector3(width, height, depth),
							Position = new Vector3(
								-width / 2f + .5f,
								0,
								+depth / 2f - .5f
							),
							RawColor = new Color24(0x00701E),
						},
					},
					Inputs = inputs,
					Outputs = outputs,
				},
				VariantPlacingRules = new PlacingRules
				{
					PrimaryEdgePositions = new Vector2[]
					{
						new Vector2(0.5f, -0.25f),
						new Vector2(0.5f, +0.25f),
					},
				},
			};
		}
	}
}
