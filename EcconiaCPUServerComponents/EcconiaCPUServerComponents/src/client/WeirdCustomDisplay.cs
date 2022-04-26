//Needed for everything else:
using LogicWorld.SharedCode;
using LogicWorld.Rendering.Components;
using LogicWorld.Interfaces.Building;
//Needed for at least 'Color24':
using JimmysUnityUtilities;
//Needed for some basic classes:
using UnityEngine;

//Needed for my components shared data:
using EcconiaCPUServerComponents.Shared;

namespace EcconiaCPUServerComponents.Client
{
	public class WeirdCustomDisplay : ComponentClientCode<IWeirdCustomDisplayData>
	{
		private bool dataDirty = true;

		private const int DisplaySideLength = 32;
		private static readonly Color24 onColor = new Color24(255, 150, 0);

		protected override void Initialize()
		{
		}

		protected override void SetDataDefaultValues()
		{
			base.Data.pixelData = new byte[128];
		}

		protected override void DataUpdate()
		{
			dataDirty = true;
			base.QueueFrameUpdate();
		}

		protected override void FrameUpdate()
		{
			if(dataDirty)
			{
				dataDirty = false;
				applyPixelData();
			}
		}

		private void applyPixelData()
		{
			int index = 0;
			foreach(byte b in base.Data.pixelData)
			{
				base.SetBlockColor((b & 0b10000000) != 0 ? onColor : Colors.DisplayOff, index++);
				base.SetBlockColor((b & 0b1000000) != 0 ? onColor : Colors.DisplayOff, index++);
				base.SetBlockColor((b & 0b100000) != 0 ? onColor : Colors.DisplayOff, index++);
				base.SetBlockColor((b & 0b10000) != 0 ? onColor : Colors.DisplayOff, index++);
				base.SetBlockColor((b & 0b1000) != 0 ? onColor : Colors.DisplayOff, index++);
				base.SetBlockColor((b & 0b100) != 0 ? onColor : Colors.DisplayOff, index++);
				base.SetBlockColor((b & 0b10) != 0 ? onColor : Colors.DisplayOff, index++);
				base.SetBlockColor((b & 0b1) != 0 ? onColor : Colors.DisplayOff, index++);
			}
		}

		private void setPixel(int x, int y, bool state)
		{
			base.SetBlockColor(state ? onColor : Colors.DisplayOff, x + y * 32);
		}

		//Allows components to be placed on this component.
		protected override ChildPlacementInfo GenerateChildPlacementInfo()
		{
			return new ChildPlacementInfo
			{
				Points = new FixedPlacingPoint[]
				{
					//The Position will always be multiplied by 0.3f.
					//UpDirection defines the rotation of the component to be placed.
					//Range is for the distance of hitpoint and actual placement point.
					new FixedPlacingPoint
					{
						Position = new Vector3(0f, (float) DisplaySideLength * 2f, 0f),
						//UpDirection = new Vector3(0f, 1f, 0f),
						Range = 100f //Pretty much any radius is allowed, there is only one position.
					},
				}
			};
		}
	}
}
