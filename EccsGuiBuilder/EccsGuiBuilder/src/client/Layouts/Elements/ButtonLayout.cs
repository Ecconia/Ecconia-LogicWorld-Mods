using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EccsGuiBuilder.Client.Layouts.Elements
{
	public class ButtonLayout : MonoBehaviour, ILayoutElement
	{
		private float height { get; set; } = 70;
		
		private TextMeshProUGUI title;
		
		private void Awake()
		{
			title = GetComponentInChildren<TextMeshProUGUI>();
			title.ForceMeshUpdate();
		}
		
		public void CalculateLayoutInputHorizontal()
		{
			minWidth = title.preferredWidth + 20; //MinWidth is broken in TMP.
			preferredWidth = title.preferredWidth + 20;
		}
		
		public void CalculateLayoutInputVertical()
		{
			//The size is constant and defined inside the title bar rect:
			minHeight = height;
			preferredHeight = height;
		}
		
		public float minWidth { get; private set; }
		public float preferredWidth { get; private set; }
		public float flexibleWidth => -1;
		public float minHeight { get; private set; }
		public float preferredHeight { get; private set;}
		public float flexibleHeight => -1;
		public int layoutPriority => 0;
	}
}
