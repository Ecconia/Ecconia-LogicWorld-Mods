using System;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using UnityEngine;

namespace FileDump.Client
{
	public class FileDumpPrefabGenerator : DynamicPrefabGenerator<(int, int)>
	{
		protected override (int, int) GetIdentifierFor(ComponentData componentData)
			=> (componentData.InputCount, componentData.OutputCount);
		
		public override (int inputCount, int outputCount) GetDefaultPegCounts()
			=> (4, 0);
		
		protected override Prefab GeneratePrefabFor((int, int) identifier)
		{
			var (inputCount, outputCount) = identifier;
			if(outputCount != 0)
			{
				throw new Exception("Attempted to create FileDump with output pegs. This component does not support output pegs. Fix your save or mod!");
			}
			if(inputCount <= 0)
			{
				throw new Exception("Attempted to create FileDump with no input pegs. This component does not support no pegs. Fix your save or mod!");
			}
			
			var baseOffsetX = inputCount / 2f;
			var inputs = new ComponentInput[inputCount];
			for(var i = 0; i < inputs.Length; i++)
			{
				inputs[i] = new ComponentInput()
				{
					Position = new Vector3(i, 0.5f, 0.25f),
					Length = 0.5f,
				};
			}
			return new Prefab()
			{
				Blocks = new Block[]
				{
					new Block()
					{
						Position = new Vector3(baseOffsetX - 0.5f, 0, 0),
						RawColor = new Color24(255, 255, 255),
						Scale = new Vector3(inputCount, 0.5f, 1),
					},
				},
				Inputs = inputs,
			};
		}
	}
}
