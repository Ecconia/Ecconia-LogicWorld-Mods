using EccsGuiBuilder.client.Layouts.Helper;
using EccsGuiBuilder.Client.Wrappers;
using LogicUI.Layouts.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace EccsGuiBuilder.Client.Layouts.Helper
{
	public static class LayoutExtension
	{
		// Unity Vertical Layout:
		
		public static RootWrapper<T> layoutVertical<T>(
			this RootWrapper<T> wrapper,
			float spacing = 20, RectOffset padding = null,
			Anchor anchor = Anchor.Start,
			bool expandChildThickness = true
		) where T: RootWrapper<T> {
			return wrapper.addAndConfigure<VerticalLayoutGroup>(layout => {
				layout.childForceExpandWidth = layout.childForceExpandHeight = expandChildThickness;
				layout.spacing = spacing;
				layout.padding = padding ?? new RectOffset(20, 20, 20, 20);
				layout.childAlignment = anchor.toTextAnchor();
			});
		}
		
		public static RootWrapper<T> layoutVerticalInner<T>(
			this RootWrapper<T> wrapper, float spacing = 20, Anchor anchor = Anchor.Start, bool expandChildThickness = true
		) where T: RootWrapper<T> {
			return wrapper.layoutVertical(spacing, new RectOffset(), anchor, expandChildThickness);
		}
		
		public static RootWrapper<T> layoutVerticalInnerCentered<T>(
			this RootWrapper<T> wrapper, float spacing = 20
		) where T: RootWrapper<T> {
			return wrapper.layoutVertical(spacing, new RectOffset(), Anchor.Center, false);
		}
		
		// Unity Horizontal Layout:
		
		public static RootWrapper<T> layoutHorizontal<T>(
			this RootWrapper<T> wrapper,
			float spacing = 20, RectOffset padding = null,
			Anchor anchor = Anchor.Start,
			bool expandChildThickness = true
		) where T: RootWrapper<T> {
			return wrapper.addAndConfigure<HorizontalLayoutGroup>(layout => {
				layout.childForceExpandWidth = layout.childForceExpandHeight = expandChildThickness;
				layout.spacing = spacing;
				layout.padding = padding ?? new RectOffset(20, 20, 20, 20);
				layout.childAlignment = anchor.toTextAnchor();
			});
		}
		
		public static RootWrapper<T> layoutHorizontalInner<T>(
			this RootWrapper<T> wrapper, float spacing = 20, Anchor anchor = Anchor.Start, bool expandChildThickness = true
		) where T: RootWrapper<T> {
			return wrapper.layoutHorizontal(spacing, new RectOffset(), anchor, expandChildThickness);
		}
		
		public static RootWrapper<T> layoutHorizontalInnerCentered<T>(
			this RootWrapper<T> wrapper, float spacing = 20
		) where T: RootWrapper<T> {
			return wrapper.layoutHorizontal(spacing, new RectOffset(), Anchor.Center, false);
		}
		
		// LW Grow Element Layout
		
		public static RootWrapper<T> layoutGrowElement<T>(
			this RootWrapper<T> wrapper,
			RectTransform.Axis axis = RectTransform.Axis.Vertical,
			float spacing = 20,
			RectOffset padding = null,
			bool expandChildThickness = true,
			Anchor anchor = Anchor.Start,
			IndexHelper elementIndex = default // Index.First
		) where T: RootWrapper<T> {
			return wrapper.addAndConfigure<GrowElementListLayout>(layout => {
				layout.LayoutAlignment = axis;
				layout.ControlChildThickness = expandChildThickness;
				layout.Spacing = spacing;
				layout.padding = padding ?? new RectOffset(20, 20, 20, 20);
				layout.childAlignment = anchor.toTextAnchor();
				layout.CountElementsFromFront = elementIndex.getCountFromFront();
				layout.setIndex(elementIndex.getIndex());
			});
		}
		
		public static RootWrapper<T> layoutGrowElementHorizontal<T>(
			this RootWrapper<T> wrapper, float spacing = 20, RectOffset padding = null, bool expandChildThickness = true, Anchor anchor = Anchor.Start, IndexHelper elementIndex = default // Index.First
		) where T: RootWrapper<T> {
			return wrapper.layoutGrowElement(RectTransform.Axis.Horizontal, spacing, padding, expandChildThickness, anchor, elementIndex);
		}
		
		public static RootWrapper<T> layoutGrowElementHorizontalInner<T>(
			this RootWrapper<T> wrapper, float spacing = 20, bool expandChildThickness = true, Anchor anchor = Anchor.Start, IndexHelper elementIndex = default // Index.First
		) where T: RootWrapper<T> {
			return wrapper.layoutGrowElement(RectTransform.Axis.Horizontal, spacing, new RectOffset(), expandChildThickness, anchor, elementIndex);
		}
		
		public static RootWrapper<T> layoutGrowElementHorizontalInnerCentered<T>(
			this RootWrapper<T> wrapper, float spacing = 20, IndexHelper elementIndex = default // Index.First
		) where T: RootWrapper<T> {
			return wrapper.layoutGrowElement(RectTransform.Axis.Horizontal, spacing, new RectOffset(), false, Anchor.Center, elementIndex);
		}
		
		public static RootWrapper<T> layoutGrowElementVerticalInner<T>(
			this RootWrapper<T> wrapper, float spacing = 20, bool expandChildThickness = true, Anchor anchor = Anchor.Start, IndexHelper elementIndex = default // Index.First
		) where T: RootWrapper<T> {
			return wrapper.layoutGrowElement(RectTransform.Axis.Vertical, spacing, new RectOffset(), expandChildThickness, anchor, elementIndex);
		}
		
		public static RootWrapper<T> layoutGrowElementVerticalInnerCentered<T>(
			this RootWrapper<T> wrapper, float spacing = 20, IndexHelper elementIndex = default // Index.First
		) where T: RootWrapper<T> {
			return wrapper.layoutGrowElement(RectTransform.Axis.Vertical, spacing, new RectOffset(), false, Anchor.Center, elementIndex);
		}
		
		// LW Grow Gap Layout:
		
		public static RootWrapper<T> layoutGrowGap<T>(
			this RootWrapper<T> wrapper,
			RectTransform.Axis axis = RectTransform.Axis.Vertical,
			float spacing = 20,
			float gapSpacing = 20,
			RectOffset padding = null,
			bool expandChildThickness = true,
			Anchor anchor = Anchor.Start,
			IndexHelper gapIndex = default // Index.First
		) where T: RootWrapper<T> {
			return wrapper.addAndConfigure<GrowGapListLayout>(layout => {
				layout.LayoutAlignment = axis;
				layout.ControlChildThickness = expandChildThickness;
				layout.Spacing = spacing;
				layout.GapSpace = gapSpacing;
				layout.padding = padding ?? new RectOffset(20, 20, 20, 20);
				layout.childAlignment = anchor.toTextAnchor();
				layout.CountElementsFromFront = gapIndex.getCountFromFront();
				layout.setIndex(gapIndex.getIndex());
			});
		}
		
		public static RootWrapper<T> layoutGrowGapHorizontal<T>(
			this RootWrapper<T> wrapper, float spacing = 20, float gapSpacing = 20, RectOffset padding = null, bool expandChildThickness = true, Anchor anchor = Anchor.Start, IndexHelper gapIndex = default // Index.First
		) where T: RootWrapper<T> {
			return wrapper.layoutGrowGap(RectTransform.Axis.Horizontal, spacing, gapSpacing, padding, expandChildThickness, anchor, gapIndex);
		}
		
		public static RootWrapper<T> layoutGrowGapHorizontalInner<T>(
			this RootWrapper<T> wrapper, float spacing = 20, float gapSpacing = 20, bool expandChildThickness = true, Anchor anchor = Anchor.Start, IndexHelper gapIndex = default // Index.First
		) where T: RootWrapper<T> {
			return wrapper.layoutGrowGap(RectTransform.Axis.Horizontal, spacing, gapSpacing, new RectOffset(), expandChildThickness, anchor, gapIndex);
		}
		
		public static RootWrapper<T> layoutGrowGapHorizontalInnerCentered<T>(
			this RootWrapper<T> wrapper, float spacing = 20, float gapSpacing = 20, IndexHelper gapIndex = default // Index.First
		) where T: RootWrapper<T> {
			return wrapper.layoutGrowGap(RectTransform.Axis.Horizontal, spacing, gapSpacing, new RectOffset(), false, Anchor.Center, gapIndex);
		}
		
		public static RootWrapper<T> layoutGrowGapVerticalInner<T>(
			this RootWrapper<T> wrapper, float spacing = 20, float gapSpacing = 20, bool expandChildThickness = true, Anchor anchor = Anchor.Start, IndexHelper gapIndex = default // Index.First
		) where T: RootWrapper<T> {
			return wrapper.layoutGrowGap(RectTransform.Axis.Vertical, spacing, gapSpacing, new RectOffset(), expandChildThickness, anchor, gapIndex);
		}
		
		public static RootWrapper<T> layoutGrowGapVerticalInnerCentered<T>(
			this RootWrapper<T> wrapper, float spacing = 20, float gapSpacing = 20, IndexHelper gapIndex = default // Index.First
		) where T: RootWrapper<T> {
			return wrapper.layoutGrowGap(RectTransform.Axis.Vertical, spacing, gapSpacing, new RectOffset(), false, Anchor.Center, gapIndex);
		}
	}
}
