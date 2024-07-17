using System;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using UnityEngine;

namespace EcconiaCPUServerComponents.Client
{
	public class Ram256b8PrefabGenerator : DynamicPrefabGenerator<(int, int)>
	{
		//Units in squares:
		public static readonly float width = 20f;
		public static readonly float depth = 481.5f;
		public static readonly float height = 13f;
		
		private static readonly int dataWidth = 8;
		private static readonly int definedOutputCount = dataWidth;
		private static readonly int definedInputCount = 9 + 9 + dataWidth;
		
		protected override (int, int) GetIdentifierFor(ComponentData componentData)
			=> (componentData.InputCount, componentData.OutputCount);
		
		public override (int inputCount, int outputCount) GetDefaultPegCounts()
			=> (definedInputCount, definedOutputCount);
		
		protected override Prefab GeneratePrefabFor((int, int) identifier)
		{
			var (inputCount, outputCount) = identifier;
			if(inputCount != definedInputCount || outputCount != definedOutputCount)
			{
				throw new Exception("Attempted to create Ecconias Ram256b8 with unexpected peg configuration. Loading old save? Wrong mod version? Inputs: " + inputCount + " Outputs: " + outputCount);
			}
			
			var inputs = new ComponentInput[definedInputCount];
			var index = 0;
			//WRITE:
			for(var i = 0; i < 9; i++)
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
			for(var i = 0; i < 9; i++)
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
			for(var i = 0; i < 8; i++)
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
			
			var outputs = new ComponentOutput[definedOutputCount];
			for(var i = 0; i < dataWidth; i++)
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
			
			return new Prefab()
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
			};
		}
	}
}
