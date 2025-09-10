using System;
using System.Reflection;
using System.Text;
using CustomWirePlacer.Client.CWP;
using EccsGuiBuilder.Client.Layouts.Helper;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicInitializable;
using LogicLocalization;
using LogicUI.MenuTypes;
using LogicWorld.UI.DebugToggleTexts;
using LogicWorld.UI.HelpList;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomWirePlacer.Client.Windows
{
	public class CWPStatusOverlay : MonoBehaviour, ILocalizedObject, IAssignMyFields, IInitializable
	{
		private static GameObject rootObject;
		
		private static bool genericDirty = true;
		
		public static void Init()
		{
			//Static initialization, create the game object, which this window is attached to.
			if(rootObject != null)
			{
				throw new Exception("Already initialized CWP-StatusDisplay");
			}
			WS.canvas("CWP: StatusDisplay root")
				.assignTo(out rootObject)
				.addContainer("CWP: StatusDisplay window", content => content
					.setAlignment(Alignment.TopLeft)
					.layoutVertical(padding: new RectOffset(5, 5, 0, 5))
					.addAndConfigure<ContentSizeFitter>(configure => {
						configure.horizontalFit = ContentSizeFitter.FitMode.MinSize;
						configure.verticalFit = ContentSizeFitter.FitMode.MinSize;
					})
					.injectionKey(nameof(windowRect))
					.addDebugBackground(new Color32(0, 0, 0, 128))
					.add(WS.textLine
						.removeLocalization()
						.injectionKey(nameof(textMesh))
						.configureTMP(tmp => {
							tmp.fontSize = 40;
							tmp.verticalAlignment = VerticalAlignmentOptions.Top;
							tmp.horizontalAlignment = HorizontalAlignmentOptions.Left;
							// tmp.autoSizeTextContainer = true;
							tmp.textWrappingMode = TextWrappingModes.PreserveWhitespaceNoWrap;
						})
					)
				)
				.add<CWPStatusOverlay>()
				.build();
		}
		
		public static void destroy()
		{
			rootObject = null;
		}
		
		public static void setVisible(bool val)
		{
			rootObject.SetActive(val);
		}
		
		[AssignMe]
		public TextMeshProUGUI textMesh;
		[AssignMe]
		public RectTransform windowRect;
		
		public void Initialize()
		{
			fieldRect = Fields.getPrivate(typeof(DebugToggleTextManager), "PlaceholdersParent");
			TextLocalizer.Initialize(this);
		}
		
		private void OnDestroy()
		{
			TextLocalizer.Finish(this);
		}
		
		private void setText(string text)
		{
			textMesh.text = text;
		}
		
		private FieldInfo fieldRect;
		private DebugToggleTextManager obj;
		private bool keyHelpOpen;
		private byte debugState;
		
		private void updatePosition(bool dirty = false)
		{
			var keyHelpOpen = ToggleableSingletonMenu<HelpListMenu>.MenuIsVisible;
			if(keyHelpOpen != this.keyHelpOpen)
			{
				this.keyHelpOpen = keyHelpOpen;
				dirty = true;
			}
			
			var debugState = (byte) ((ToggleableSingletonMenu<DebugFpsText>.MenuIsVisible ? 2 : 0) | (ToggleableSingletonMenu<DebugCoordinatesText>.MenuIsVisible ? 1 : 0));
			if(debugState != this.debugState)
			{
				this.debugState = debugState;
				dirty = true;
			}
			
			if(dirty)
			{
				actuallyUpdatePosition();
			}
		}
		
		private void actuallyUpdatePosition()
		{
			if(keyHelpOpen)
			{
				windowRect.anchoredPosition = new Vector2(562, 0);
			}
			else if(fieldRect != null)
			{
				if(obj == null)
				{
					obj = FindFirstObjectByType<DebugToggleTextManager>();
					if(obj == null)
					{
						ModClass.logger.Error("Could not find instance of 'DebugToggleTextManager', cannot set status display at right position when debugging window.");
						fieldRect = null; //Disable.
						windowRect.anchoredPosition = new Vector2(0, 0);
						return;
					}
				}
				var rectTransform = (RectTransform) fieldRect.GetValue(obj);
				windowRect.anchoredPosition = new Vector2(0, -rectTransform.sizeDelta.y);
			}
			else
			{
				windowRect.anchoredPosition = new Vector2(0, 0);
			}
		}
		
		private void OnEnable()
		{
			updatePosition(true);
		}
		
		private void Update()
		{
			if(!CWP.CustomWirePlacer.isActive())
			{
				setText(TextLocalizer.LocalizeByKey("CWP.StatusOverlay.Off"));
				return;
			}
			updatePosition();
			if(genericDirty)
			{
				constructText();
				genericDirty = false;
			}
		}
		
		private void constructText()
		{
			var firstGroup = CWP.CustomWirePlacer.getFirstGroup();
			var secondGroup = CWP.CustomWirePlacer.getSecondGroup();
			var currentGroup = CWP.CustomWirePlacer.getCurrentGroup();
			var sb = new StringBuilder();
			//Peg count:
			if(firstGroup.isSet())
			{
				if(firstGroup.isTwoDimensional())
				{
					sb.Append(TextLocalizer.LocalizedFormat("CWP.StatusOverlay.Dim2Pegs", 1, firstGroup.getPegCount(),
						firstGroup.getFirstAxis().getPegCount(), firstGroup.getSecondAxis().getPegCount()));
				}
				else
				{
					sb.Append(TextLocalizer.LocalizedFormat("CWP.StatusOverlay.Dim1Pegs", 1, firstGroup.getPegCount()));
				}
				sb.Append('\n');
			}
			if(secondGroup.isSet())
			{
				if(secondGroup.isTwoDimensional())
				{
					sb.Append(TextLocalizer.LocalizedFormat("CWP.StatusOverlay.Dim2Pegs", 2, secondGroup.getPegCount(),
						secondGroup.getFirstAxis().getPegCount(), secondGroup.getSecondAxis().getPegCount()));
				}
				else
				{
					sb.Append(TextLocalizer.LocalizedFormat("CWP.StatusOverlay.Dim1Pegs", 2, secondGroup.getPegCount()));
				}
				sb.Append('\n');
			}
			
			//Current axis details:
			var currentAxis = currentGroup.getCurrentAxis();
			var forward = currentAxis.forwards != null ? currentAxis.forwards.Count : 0;
			var backward = currentAxis.backwards != null ? currentAxis.backwards.Count : 0;
			if(forward != 0 || backward != 0)
			{
				sb.Append(TextLocalizer.LocalizedFormat("CWP.StatusOverlay.Expand", backward, forward)).Append('\n');
			}
			
			//Skipping:
			var amount = currentGroup.getCurrentAxis().skipNumber;
			if(amount != 0)
			{
				if(currentGroup.getCurrentAxis().binarySkipping)
				{
					sb.Append(TextLocalizer.LocalizedFormat("CWP.StatusOverlay.SkipStateGroup", amount)).Append('\n');
				}
				else
				{
					sb.Append(TextLocalizer.LocalizedFormat("CWP.StatusOverlay.SkipStateNormal", amount, (amount == 1 ? "" : "s"))).Append('\n');
				}
				if(currentGroup.getCurrentAxis().skipOffset != 0)
				{
					sb.Append(TextLocalizer.LocalizedFormat("CWP.StatusOverlay.SkipOffset", currentGroup.getCurrentAxis().skipOffset)).Append('\n');
				}
			}
			
			//Modifiers:
			if(CWPSettings.showStatusOverlayModifiers)
			{
				if(CWP.CustomWirePlacer.flipping)
				{
					sb.Append(TextLocalizer.LocalizeByKey("CWP.StatusOverlay.Flipping")).Append('\n');
				}
				if(CWP.CustomWirePlacer.pendingTwoDimensional)
				{
					sb.Append(TextLocalizer.LocalizeByKey("CWP.StatusOverlay.TwoDim")).Append('\n');
				}
				if(CWP.CustomWirePlacer.waitForPegToApplyPatternTo)
				{
					sb.Append(TextLocalizer.LocalizeByKey("CWP.StatusOverlay.Pattern")).Append('\n');
				}
			}
			
			//Settings:
			if(CWPSettings.showStatusOverlaySettings)
			{
				if(CWPSettings.raycastAtBottomOfPegs)
				{
					sb.Append(TextLocalizer.LocalizeByKey("CWP.StatusOverlay.RaycastBottom")).Append('\n');
				}
				if(CWPSettings.expandOnlyUniformDistance)
				{
					sb.Append(TextLocalizer.LocalizeByKey("CWP.StatusOverlay.ExpandUniform")).Append('\n');
				}
			}
			
			setText(sb.ToString());
		}
		
		public static void setDirtyGeneric()
		{
			genericDirty = true;
		}
		
		public static void setDirtySettings()
		{
			setDirtyGeneric();
		}
		
		public static void setDirtySettingsConfig()
		{
			setDirtyGeneric();
		}
		
		public void UpdateLocalization()
		{
			setDirtyGeneric();
		}
	}
}
