using UnityEngine;
using UnityEngine.UI;

namespace EccsGuiBuilder.Client.Layouts.Elements
{
	public class DefinedSizeLayout : MonoBehaviour, ILayoutElement
	{
		public Vector2 size = Vector2.zero;
		
		public void CalculateLayoutInputHorizontal()
		{
		}
		
		public void CalculateLayoutInputVertical()
		{
		}
		
		public float minWidth => size[0];
		public float preferredWidth => size[0];
		public float flexibleWidth => -1;
		public float minHeight => size[1];
		public float preferredHeight => size[1];
		public float flexibleHeight => -1;
		public int layoutPriority => 4; //Has to be higher than TMPFixer
	}
}
