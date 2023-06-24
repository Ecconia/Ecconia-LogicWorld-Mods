using LICC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EccsGuiBuilder.Client.Layouts.Elements
{
	public class TMPFixer : MonoBehaviour, ILayoutElement
	{
		private TextMeshProUGUI title;
		
		private void Awake()
		{
			title = GetComponent<TextMeshProUGUI>();
			title.ForceMeshUpdate();
		}
		
		public void CalculateLayoutInputHorizontal()
		{
			minWidth = title.preferredWidth; //MinWidth is broken in TMP.
			preferredWidth = title.preferredWidth;
		}
		
		public void CalculateLayoutInputVertical()
		{
			minHeight = title.preferredHeight;
			preferredHeight = title.preferredHeight;
		}
		
		public float minWidth { get; private set; }
		public float preferredWidth { get; private set; }
		public float flexibleWidth => -1;
		public float minHeight { get; private set; }
		public float preferredHeight { get; private set;}
		public float flexibleHeight => -1;
		public int layoutPriority => 2; //Has to be higher than TMP
	}
}
