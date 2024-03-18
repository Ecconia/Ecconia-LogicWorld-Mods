using ComponentActionExampleMod.shared.ex6;
using JimmysUnityUtilities;
using LogicAPI.Data;
using LogicWorld.Rendering.Components;
using UnityEngine;

namespace ComponentActionExampleMod.Client.Raw.Ex6
{
	public class EditableBlock : ComponentClientCode
	{
		// Parsed custom data, here for easy access, management is manual:
		public float currentHeight;
		public Color24 currentColor;
		
		protected override void Initialize()
		{
			if (Component.Data.CustomData == null)
			{
				// Set defaults:
				setColor(Color24.Orange);
				setHeight(1f);
				((IEditableComponentData) Component.Data).CustomData = EditableComponentCustomData.getCustomDataFor(currentHeight, currentColor);
			}
			else
			{
				// Load stored:
				var (height, color) = EditableComponentCustomData.parseCustomData(Component.Data.CustomData);
				setHeight(height);
				setColor(color);
			}
		}
		
		public void setColor(Color24 color)
		{
			currentColor = color;
			SetBlockColor(currentColor.ToGpuColor(), 0);
		}
		
		public void setHeight(float height)
		{
			currentHeight = height;
			SetBlockScale(0, new Vector3(1f, currentHeight, 1f));
		}
	}
}
