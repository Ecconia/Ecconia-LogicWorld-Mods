using LogicWorld.Interfaces.Building;
using LogicWorld.Rendering.Components;
using UnityEngine;

namespace EcconiaCPUServerComponents.Client
{
	public class Ram256b8 : ComponentClientCode
	{
		protected override ChildPlacementInfo GenerateChildPlacementInfo()
		{
			return new ChildPlacementInfo()
			{
				Points = new FixedPlacingPoint[]
				{
					new FixedPlacingPoint()
					{
						Position = new Vector3(0f, Ram256b8Prefab.height, 0f),
						Range = 100f,
					},
				},
			};
		}
	}
}
