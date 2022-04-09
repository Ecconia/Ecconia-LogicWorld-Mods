using System;
using JimmysUnityUtilities;
using LogicWorld.Interfaces;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using UnityEngine;

namespace EcconiaCPUServerComponents.Client
{
	public class TPSPrinter : PrefabVariantInfo
	{
		private const int OutputBitwidth = 16;

		public override string ComponentTextID => "EcconiaCPUServerComponents.TPSPrinter";

		public override PrefabVariantIdentifier GetDefaultComponentVariant()
		{
			return new PrefabVariantIdentifier(0, OutputBitwidth);
		}

		public override ComponentVariant GenerateVariant(PrefabVariantIdentifier identifier)
		{
			if(identifier.InputCount != 0 || identifier.OutputCount != OutputBitwidth)
			{
				throw new Exception("Attempted to create TPSPrinter with unexpected peg configuration. Loading old save? Wrong mod version?");
			}

			//Inputs:
			ComponentOutput[] outputs = new ComponentOutput[OutputBitwidth];
			{
				for(int i = 0; i < OutputBitwidth; i++)
				{
					outputs[i] = new ComponentOutput()
					{
						Position = new Vector3(-i, 0.5f, 0.5f),
						Rotation = new Vector3(90, 0, 0),
					};
				}
			}
			return new ComponentVariant
			{
				VariantPrefab = new Prefab
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
				},
			};
		}
	}
}
