using EccsLogicWorldAPI.Client.UnityHelper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EccsGuiBuilder.Client.Layouts.Elements
{
	public class TitleBarLayout : MonoBehaviour, ILayoutElement
	{
		public float height { get; set; } = 70 - 3;
		
		private TextMeshProUGUI title;
		private ILayoutElement buttons;
		 
		private void Awake()
		{
			title = gameObject.getChild(0).GetComponent<TextMeshProUGUI>();
			buttons = gameObject.getChild(1).GetComponent<ILayoutElement>();
		}
		
		public void CalculateLayoutInputHorizontal()
		{
			var padding = 30f;
			//As this is a shortcut layout, a fixed padding is applied - it must match what is defined in the rect!
			minWidth = title.preferredWidth + padding + buttons.minWidth; //title.minWidth; MinWidth is broken or misconfigured, it will always return 0, not helping.
			preferredWidth = title.preferredWidth + padding + buttons.preferredWidth;
		}
		
		public void CalculateLayoutInputVertical()
		{
			//Nothing to do, height is constant and defined by this layout.
		}
		
		public float minWidth { get; private set; }
		public float preferredWidth { get; private set; }
		public float flexibleWidth => -1;
		public float minHeight => height;
		public float preferredHeight => height;
		public float flexibleHeight => -1;
		public int layoutPriority => 0;
	}
}
