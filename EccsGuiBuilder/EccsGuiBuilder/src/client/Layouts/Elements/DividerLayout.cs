using UnityEngine;
using UnityEngine.UI;

namespace EccsGuiBuilder.Client.Layouts.Elements
{
	public class DividerLayout : MonoBehaviour, ILayoutElement
	{
		//TODO: Figure out whatever the default is, that LW uses.
		public float thickness { get; set; } = 7;
		
		public void CalculateLayoutInputHorizontal()
		{
		}
		
		public void CalculateLayoutInputVertical()
		{
		}
		
		public float minWidth => thickness;
		public float preferredWidth => -1;
		public float flexibleWidth => -1;
		public float minHeight => thickness;
		public float preferredHeight => -1;
		public float flexibleHeight => -1;
		public int layoutPriority => 0;
	}
}
