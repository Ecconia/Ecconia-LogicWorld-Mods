using System;
using JimmysUnityUtilities;
using LogicWorld.Interfaces;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using UnityEngine;

namespace RadioConnection.Client
{
	public class RadioComponentPrefab : PrefabVariantInfo
	{
		public override string ComponentTextID => "RadioConnection.RadioComponent";
		
		public override PrefabVariantIdentifier GetDefaultComponentVariant()
		{
			return new PrefabVariantIdentifier(4, 0);
		}
		
		public override ComponentVariant GenerateVariant(PrefabVariantIdentifier identifier)
		{
			if(identifier.OutputCount != 0)
			{
				throw new Exception("Attempted to create RadioComponent with output pegs. This component does not support output pegs. Fix your save or mod!");
			}
			
			var inputs = new ComponentInput[identifier.InputCount];
			for(int i = 0; i < inputs.Length; i++)
			{
				inputs[i] = new ComponentInput()
				{
					Position = new Vector3(),
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
							RawColor = new Color24(100, 25, 200),
						},
					},
					Inputs = inputs,
				},
			};
		}
	}
}
