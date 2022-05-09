using System;
using LICC;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CustomWirePlacer.Client.Windows
{
	public class ClickableSurface : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public void OnPointerClick(PointerEventData eventData)
		{
			LConsole.WriteLine("Clicked!");
			Debug.Log("Clicked.");
		}

		private void OnMouseDown()
		{
			LConsole.WriteLine("Mouse down.");
			Debug.Log("Down.");
		}
	}
}
