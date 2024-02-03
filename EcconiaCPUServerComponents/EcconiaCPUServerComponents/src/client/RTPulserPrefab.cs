using System;
using JimmysUnityUtilities;
using LogicWorld.Interfaces;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using UnityEngine;

namespace EcconiaCPUServerComponents.Client
{
	public class RTPulser : PrefabVariantInfo
	{
		public const string COMPONENT_TEXT_ID = "EcconiaCPUServerComponents.RTPulser";
		
		public override string ComponentTextID => COMPONENT_TEXT_ID;
		
		public override PrefabVariantIdentifier GetDefaultComponentVariant()
		{
			return new PrefabVariantIdentifier(1, 2);
		}
		
		public override ComponentVariant GenerateVariant(PrefabVariantIdentifier identifier)
		{
			if(identifier.InputCount != 1 || identifier.OutputCount != 2)
			{
				throw new Exception("Attempted to create RTPulser with unexpected peg configuration. Loading old save? Wrong mod version?");
			}
			
			return new ComponentVariant
			{
				VariantPrefab = new Prefab
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
				},
			};
		}
	}
}
