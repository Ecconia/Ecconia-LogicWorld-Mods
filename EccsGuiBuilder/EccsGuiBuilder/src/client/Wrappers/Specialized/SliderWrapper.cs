using LogicUI.MenuParts;
using UnityEngine;

namespace EccsGuiBuilder.Client.Wrappers.Specialized
{
	public class SliderWrapper : Wrapper<SliderWrapper>
	{
		private readonly InputSlider inputSlider;
		
		public SliderWrapper(GameObject gameObject) : base(gameObject)
		{
			inputSlider = gameObject.GetComponent<InputSlider>();
		}
		
		public SliderWrapper setInterval(float interval)
		{
			inputSlider.SliderInterval = interval;
			return this;
		}
		
		public SliderWrapper setMin(float min)
		{
			inputSlider.Min = min;
			return this;
		}
		
		public SliderWrapper setMax(float max)
		{
			inputSlider.Max = max;
			return this;
		}
		
		public SliderWrapper setDecimalDigitsToDisplay(int decimalDigits)
		{
			inputSlider.DecimalPlacesToDisplay = decimalDigits;
			return this;
		}
	}
}
