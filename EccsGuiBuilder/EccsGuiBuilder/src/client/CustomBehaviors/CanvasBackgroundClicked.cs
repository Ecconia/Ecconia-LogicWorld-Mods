using System;
using LogicSettings;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EccsGuiBuilder.Client.CustomBehaviors
{
	public class CanvasBackgroundClicked : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public event Action onCanvasClicked;
		public bool allowClickingToClose = true;
		
		public void OnPointerClick(PointerEventData eventData)
		{
			//Queries the settings everytime the mouse clicks somewhere, should not be too bad of an overhead:
			if(!allowClickingToClose || !(bool) SettingsManager.Instance.GetSettingValue("MHG.QuickCloseMenus"))
			{
				return;
			}
			onCanvasClicked?.Invoke();
		}
	}
}
