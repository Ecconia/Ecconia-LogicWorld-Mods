using System;
using LICC;
using UnityEngine;
using UnityEngine.UI;

namespace EccsGuiBuilder.Client.Layouts.Controller
{
	public class GapListLayout : LayoutGroup
	{
		public RectTransform.Axis layoutAlignment = RectTransform.Axis.Vertical;
		public bool countElementsFromFront = true;
		public int elementsUntilGap = 1;
		public bool reverseChildOrder;
		public bool expandChildThickness;
		
		public bool debugChildComponentSize;
		
		public float spacing;
		public float gapSpace;
		
		//Internal:
		
		private Vector2[] sizeChildren;
		
		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal(); //Has to be called, for setup reasons.
			sizeChildren = new Vector2[rectChildren.Count];
			setupChildren(RectTransform.Axis.Horizontal);
			calcForAxis(RectTransform.Axis.Horizontal);
		}
		
		public override void CalculateLayoutInputVertical()
		{
			setupChildren(RectTransform.Axis.Vertical);
			calcForAxis(RectTransform.Axis.Vertical);
		}
		
		private void calcForAxis(RectTransform.Axis axis)
		{
			var target = layoutAlignment == axis
				? findTotal(axis)
				: findMax(axis);
			target += axis == RectTransform.Axis.Horizontal ? padding.horizontal : padding.vertical;
			SetLayoutInputForAxis(target, target, -1, (int) axis);
		}
		
		private void setupChildren(RectTransform.Axis axis)
		{
			int axisIndex = (int) axis;
			for(int i = 0; i < rectChildren.Count; i++)
			{
				var childRect = rectChildren[i];
				var childLayout = childRect.GetComponent<ILayoutElement>(); //Assume there is only one - can be updated later.
				float value = -1;
				if(childLayout != null)
				{
					//Use the utility class for this, to handle the priorities. It is not the most efficient but gets the job done.
					value = LayoutUtility.GetPreferredSize(childRect, axisIndex);
				}
				//Sadly the above method calls ensure that a value is at least 0... We cannot have that.
				if(value > 0)
				{
					sizeChildren[i][axisIndex] = value;
					continue;
				}
				//Did not get the full size:
				if(value < 0)
				{
					if(Math.Abs(childRect.anchorMin[axisIndex] - childRect.anchorMax[axisIndex]) > 0.00001f)
					{
						if(layoutAlignment == axis) //Looking for axis with the same as the layout is directed in...
						{
							//Ehm... variable length list? Nah I do not think so... Just force it to something
							sizeChildren[i][axisIndex] =  24.2424f; //Recognizable. TODO: Ideally complain...
						}
						else //Looking for axis that is not the same as the layout, so a max value is considered - ignore
						{
							//Max width is important, the component follows parent. No priority set flag:
							sizeChildren[i][axisIndex] =  -1;
						}
					}
					else
					{
						sizeChildren[i][axisIndex] = childRect.sizeDelta[axisIndex];  //We got a fixed axis, just use that.
					}
				}
			}
			
			if(debugChildComponentSize)
			{
				var a = LConsole.BeginLine();
				a.WriteLine("ChildSize for " + axis);
				for(int i = 0; i < sizeChildren.Length; i++)
				{
					a.WriteLine($"- {sizeChildren[i][axisIndex]}");
				}
				a.End();
			}
		}
		
		private float findMax(RectTransform.Axis axis)
		{
			float max = -1;
			foreach(var childSize in sizeChildren)
			{
				var value = childSize[(int) axis];
				if(value > max)
				{
					max = value;
				}
			}
			return max;
		}
		
		private float findTotal(RectTransform.Axis axis)
		{
			//Spacing:
			int gapEdges = elementsUntilGap > 0 && elementsUntilGap < sizeChildren.Length ? 1 : 0;
			float total = gapSpace; //Assume there always to be the gap to be filled of minimal size
			total += (sizeChildren.Length - 1 - gapEdges) * spacing;
			//Elements:
			foreach(var childSize in sizeChildren)
			{
				var value = childSize[(int) axis];
				if(value < 0)
				{
					continue; //Some children have an invalid size (-1), these are just not added.
				}
				total += value;
			}
			//Got total:
			return total;
		}
		
		public override void SetLayoutHorizontal()
		{
			setLayout(RectTransform.Axis.Horizontal);
		}
		
		public override void SetLayoutVertical()
		{
			setLayout(RectTransform.Axis.Vertical);
		}
		
		private void setLayout(RectTransform.Axis axis)
		{
			if(layoutAlignment == axis)
			{
				align(axis);
			}
			else
			{
				adjustThickness(axis);
			}
		}
		
		private void adjustThickness(RectTransform.Axis axis)
		{
			float availableSize = layoutAlignment == RectTransform.Axis.Vertical ? rectTransform.rect.width : rectTransform.rect.height;
			//Start = V: Left | H: Top
			//End = V: Right | H: Bottom
			float paddingStart = layoutAlignment == RectTransform.Axis.Vertical ? padding.left : padding.top;
			float paddingEnd = layoutAlignment == RectTransform.Axis.Vertical ? padding.right : padding.bottom;
			//0 = Start
			//1 = Center
			//2 = End
			int alignment = layoutAlignment == RectTransform.Axis.Vertical
				? (childAlignment == TextAnchor.LowerLeft || childAlignment == TextAnchor.MiddleLeft ||childAlignment == TextAnchor.UpperLeft) ? 0
					: (childAlignment == TextAnchor.LowerCenter || childAlignment == TextAnchor.MiddleCenter ||childAlignment == TextAnchor.UpperCenter) ? 1 : 2
				: (childAlignment == TextAnchor.UpperLeft || childAlignment == TextAnchor.UpperCenter ||childAlignment == TextAnchor.UpperRight) ? 0
					: (childAlignment == TextAnchor.MiddleLeft || childAlignment == TextAnchor.MiddleCenter ||childAlignment == TextAnchor.MiddleRight) ? 1 : 2;
			float maxChildWidth = availableSize - paddingStart - paddingEnd;
			
			int iAxis = (int) axis;
			for(int i = 0; i < sizeChildren.Length; i++)
			{
				var childSize = sizeChildren[i][iAxis];
				if(childSize < 0)
				{
					continue; //This child takes care of its own thickness.
				}
				if(expandChildThickness)
				{
					childSize = maxChildWidth;
				}
				var childRect = rectChildren[i];
				SetChildAlongAxis(
					childRect, 
					(int) axis, 
					alignment == 0
						? paddingStart
						: alignment == 1
							? (availableSize - paddingEnd - childSize + paddingStart) / 2f
							: availableSize - childSize - paddingEnd,
					childSize
				);
			}
		}
		
		private void align(RectTransform.Axis axis)
		{
			float paddingOffset = layoutAlignment == RectTransform.Axis.Horizontal ? padding.left : padding.top;
			float availableSize = layoutAlignment == RectTransform.Axis.Vertical ? rectTransform.rect.height : rectTransform.rect.width;
			
			var childRects = rectChildren.ToArray(); //Copy
			var childSizes = sizeChildren;
			//If the order is reverse, treat the object tree as if all the children are the other way round.
			//This is useful as a compromise when the overlapping oder should be reversed.
			if(reverseChildOrder)
			{
				Array.Reverse(childRects);
				var inPlace = new Vector2[childSizes.Length];
				Array.Copy(childSizes, inPlace, childSizes.Length);
				childSizes = inPlace;
				Array.Reverse(childSizes);
			}
			
			//Calculate the gap index:
			var gapIndex = elementsUntilGap < 0 ? 0 : elementsUntilGap > childSizes.Length ? childSizes.Length : elementsUntilGap;
			if(!countElementsFromFront)
			{
				gapIndex = childSizes.Length - gapIndex;
			}
			float overflow = availableSize - GetTotalPreferredSize((int) axis);
			if(overflow < 0)
			{
				//Whoops, something went horribly wrong... Probably misuse or failed configuration of layout.
				overflow = 0;
			}
			var gapSize = overflow + gapSpace;
			
			float totalOffset = paddingOffset;
			for(int i = 0; i < childSizes.Length; i++)
			{
				if(i == gapIndex)
				{
					totalOffset += gapSize;
				}
				var rect = childRects[i];
				var size = childSizes[i][(int) axis];
				SetChildAlongAxis(rect, (int) axis, totalOffset, size);
				totalOffset += size;
				if(i != gapIndex - 1)
				{
					totalOffset += spacing;
				}
			}
		}
	}
}
