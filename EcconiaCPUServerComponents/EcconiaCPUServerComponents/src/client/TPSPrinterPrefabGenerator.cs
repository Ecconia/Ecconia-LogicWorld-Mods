using System;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using UnityEngine;

namespace EcconiaCPUServerComponents.Client
{
	public class TPSPrinterPrefabGenerator : DynamicPrefabGenerator<(int, int)>
	{
		private const int OutputBitwidth = 16;
		
		protected override (int, int) GetIdentifierFor(ComponentData componentData)
			=> (componentData.InputCount, componentData.OutputCount);
		
		public override (int inputCount, int outputCount) GetDefaultPegCounts()
			=> (0, OutputBitwidth);
		
		protected override Prefab GeneratePrefabFor((int, int) identifier)
		{
			var (inputCount, outputCount) = identifier;
			if(inputCount != 0)
			{
				throw new Exception("Attempted to create TPSPrinter with input pegs. Fix your save (or mod)!");
			}
			if(outputCount != OutputBitwidth)
			{
				throw new Exception("Attempted to create TPSPrinter with unexpected peg configuration. Loading old save? Wrong mod version?");
			}
			
			//Inputs:
			var outputs = new ComponentOutput[OutputBitwidth];
			{
				for(var i = 0; i < OutputBitwidth; i++)
				{
					outputs[i] = new ComponentOutput()
					{
						Position = new Vector3(-i, 0.5f, 0.5f),
						Rotation = new Vector3(90, 0, 0),
					};
				}
			}
			return new Prefab
			{
				Blocks = new Block[]
				{
					new Block()
					{
						Scale = new Vector3(OutputBitwidth, 1, 1),
						Position = new Vector3(-OutputBitwidth / 2f + 0.5f, 0, 0),
						RawColor = new Color24(0xFF8800),
					},
				},
				Outputs = outputs,
			};
		}
	}
}
