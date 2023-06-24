using UnityEngine;
using UnityEngine.UI;

namespace EccsGuiBuilder.Client.Layouts.Elements
{
	public class TitleBarButtonLayout : MonoBehaviour, ILayoutElement
	{
		//TODO: Figure out whatever the default is, that LW uses.
		public float side { get; set; } = 50;
		
		public void CalculateLayoutInputHorizontal()
		{
		}
		
		public void CalculateLayoutInputVertical()
		{
		}
		
		public float minWidth => side;
		public float preferredWidth => -1;
		public float flexibleWidth => -1;
		public float minHeight => side;
		public float preferredHeight => -1;
		public float flexibleHeight => -1;
		public int layoutPriority => 0;
	}
}
