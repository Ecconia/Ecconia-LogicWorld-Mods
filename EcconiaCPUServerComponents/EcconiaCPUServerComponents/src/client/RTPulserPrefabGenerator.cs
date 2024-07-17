using System;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using UnityEngine;

namespace EcconiaCPUServerComponents.Client
{
	public class RTPulserPrefabGenerator : DynamicPrefabGenerator<(int, int)>
	{
		protected override (int, int) GetIdentifierFor(ComponentData componentData)
			=> (componentData.InputCount, componentData.OutputCount);
		
		public override (int inputCount, int outputCount) GetDefaultPegCounts()
			=> (1, 2);
		
		protected override Prefab GeneratePrefabFor((int, int) identifier)
		{
			var (inputCount, outputCount) = identifier;
			if(inputCount != 1 || outputCount != 2)
			{
				throw new Exception("Attempted to create RTPulser with unexpected peg configuration. Loading old save? Wrong mod version?");
			}
			
			return new Prefab
			{
				Blocks = new Block[]
				{
					new Block()
					{
						Scale = new Vector3(1, 1, 0.5f),
						RawColor = new Color24(0xF6D32D),
					},
				},
				Outputs = new ComponentOutput[]
				{
					//Top output, for pulse-return:
					new ComponentOutput()
					{
						Rotation = new Vector3(45f, 0, 0),
						Position = new Vector3(0, 0.8f, 0),
					},
					//Bottom output, for generated RT-Pulse:
					new ComponentOutput()
					{
						Rotation = new Vector3(90f, 0, 0),
						Position = new Vector3(0, 0.5f, 0.17f),
					},
				},
				Inputs = new ComponentInput[]
				{
					new ComponentInput()
					{
						Rotation = new Vector3(-45f, 0, 0),
						Position = new Vector3(0, 0.8f, 0),
					},
				},
			};
		}
	}
}
