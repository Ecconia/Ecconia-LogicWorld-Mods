using System;
using LogicWorld.Interfaces;
using LogicWorld.References;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using UnityEngine;

namespace EcconiaCPUServerComponents.Client
{
	public class FlatKeyPrefab : PrefabVariantInfo
	{
		public override string ComponentTextID => "EcconiaCPUServerComponents.FlatKey";

		public override PrefabVariantIdentifier GetDefaultComponentVariant()
		{
			return new PrefabVariantIdentifier(0, 1);
		}

		public override ComponentVariant GenerateVariant(PrefabVariantIdentifier identifier)
		{
			if(identifier.InputCount != 0 || identifier.OutputCount != 1)
			{
				throw new Exception("Attempted to create Ecconias FlatKey with unexpected peg configuration. Loading old save? Wrong mod version? Inputs: " + identifier.InputCount + " Outputs: " + identifier.OutputCount);
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
							Scale = new Vector3(1, 1f / 3f, 1),
						},
						new Block()
						{
							Rotation = new Vector3(180, 0, 0),
							Scale = new Vector3(2f / 3f, 5f / 6f, 2f / 3f),
							Mesh = Meshes.BetterCube_OpenBottom,

							ColliderData = new ColliderData()
							{
								Type = ColliderType.Box,
								Transform = new ColliderTransform()
								{
									LocalPosition = new Vector3(0, 0.8f, 0),
									LocalScale = new Vector3(1, 0.4f, 1),
								},
							},
						},
					},
					Outputs = new ComponentOutput[]
					{
						new ComponentOutput()
						{
							Position = new Vector3(0, -5f/6f,0 ),
							Rotation = new Vector3(180, 0, 0),
						},
					},
				},
			};
		}
	}
}
