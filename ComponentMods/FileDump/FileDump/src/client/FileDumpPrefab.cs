using System;
using JimmysUnityUtilities;
using LogicWorld.Interfaces;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using UnityEngine;

namespace FileDump.Client
{
	public class FileDumpPrefab : PrefabVariantInfo
	{
		public override string ComponentTextID => "FileDump.FileDump";
		
		public override PrefabVariantIdentifier GetDefaultComponentVariant()
		{
			return new PrefabVariantIdentifier(4, 0);
		}
		
		public override ComponentVariant GenerateVariant(PrefabVariantIdentifier identifier)
		{
			if(identifier.OutputCount != 0)
			{
				throw new Exception("Attempted to create FileDump with output pegs. This component does not support output pegs. Fix your save or mod!");
			}
			if(identifier.InputCount <= 0)
			{
				throw new Exception("Attempted to create FileDump with no input pegs. This component does not support no pegs. Fix your save or mod!");
			}
			
			var baseOffsetX = identifier.InputCount / 2f;
			var inputs = new ComponentInput[identifier.InputCount];
			for(var i = 0; i < inputs.Length; i++)
			{
				inputs[i] = new ComponentInput()
				{
					Position = new Vector3(i, 0.5f, 0.25f),
					Length = 0.5f,
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
							Position = new Vector3(baseOffsetX - 0.5f, 0, 0),
							RawColor = new Color24(255, 255, 255),
							Scale = new Vector3(identifier.InputCount, 0.5f, 1),
						},
					},
					Inputs = inputs,
				},
			};
		}
	}
}
