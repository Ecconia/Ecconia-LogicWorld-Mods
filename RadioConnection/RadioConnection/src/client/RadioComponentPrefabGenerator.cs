using System;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using UnityEngine;

namespace RadioConnection.Client
{
	public class RadioComponentPrefabGenerator : DynamicPrefabGenerator<(int, int)>
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
				throw new Exception("Attempted to create RadioComponent with output pegs. This component does not support output pegs. Fix your save or mod!");
			}
			if(inputCount < 0)
			{
				throw new Exception("Attempted to create RadioComponent with negative input peg count. Fix your save or mod!");
			}
			
			var inputs = new ComponentInput[inputCount];
			for(var i = 0; i < inputs.Length; i++)
			{
				inputs[i] = new ComponentInput()
				{
					Position = new Vector3(),
				};
			}
			return new Prefab()
			{
				Blocks = new Block[]
				{
					new Block()
					{
						RawColor = new Color24(100, 25, 200),
					},
				},
				Inputs = inputs,
			};
		}
	}
}
