using System;
using System.Collections.Generic;
using EccsGuiBuilder.Client.Layouts.Helper;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using LogicAPI.Data.BuildingRequests;
using LogicUI.MenuParts;
using LogicWorld.BuildingManagement;
using LogicWorld.UI;
using UnityEngine;

namespace EcconiaCPUServerComponentsGui.Client.EditGUI
{
	public class EditRTPulser : EditComponentMenu, IAssignMyFields
	{
		public static void initialize()
		{
			WS.window("EccComponentsEditRTPulserWindow")
				.setYPosition(120)
				.configureContent(content => content
					.layoutVertical(spacing: 10, padding: new RectOffset(20, 20, 10, 20)) // Make the gap between label and element smaller and compensate for text-space at the top.
					.add(WS.textLine
						.setLocalizationKey("EcconiaCPUServerComponents.Gui.RTPulser.Label")
						.setFontSize(40f)
					)
					.add(WS.slider
						.injectionKey(nameof(valueSlider))
						.setMin(0.001f)
						.setInterval(0.001f)
						.setMax(30f)
						.setDecimalDigitsToDisplay(3)
						.fixedSize(500, 45) //Min width is defined here...
					)
				)
				.add<EditRTPulser>()
				.build();
		}
		
		//Instance part:
		
		[AssignMe]
		public InputSlider valueSlider;
		
		public override void Initialize()
		{
			base.Initialize();
			
			valueSlider.OnValueChanged += value => {
				var millis = (int) (value * 1000.0);
				foreach(var component in ComponentsBeingEdited)
				{
					BuildRequestManager.SendBuildRequestWithoutAddingToUndoStack(new BuildRequest_UpdateComponentCustomData(
						component.Address,
						BitConverter.GetBytes(millis)
					));
				}
			};
		}
		
		protected override IEnumerable<string> GetTextIDsOfComponentTypesThatCanBeEdited()
		{
			return new string[]
			{
				"EcconiaCPUServerComponents.RTPulser",
			};
		}
		
		private float? getTime()
		{
			var data = FirstComponentBeingEdited.Component.Data.CustomData;
			if(data == null)
			{
				return null;
			}
			if(data.Length == 4)
			{
				return BitConverter.ToInt32(data, 0) / 1000f;
			}
			if(data.Length == 9)
			{
				return BitConverter.ToInt32(data, 1) / 1000f;
			}
			return null;
		}
		
		protected override void OnStartEditing()
		{
			try
			{
				var time = getTime();
				if(time != null)
				{
					valueSlider.SetValueWithoutNotify((float) time);
					return;
				}
			}
			catch(Exception e)
			{
				ModClass.logger.Error("Was not able to get delay of RTPulser component to display in gui. See exception:");
				ModClass.logger.Error(e.ToString());
			}
			//Default value:
			valueSlider.SetValueWithoutNotify(1f);
		}
	}
}
