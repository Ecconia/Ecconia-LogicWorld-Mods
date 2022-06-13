using System;
using System.Reflection;
using System.Text;
using CustomWirePlacer.Client.CWP;
using EccsWindowHelper.Client;
using LogicLocalization;
using LogicUI.MenuTypes;
using LogicWorld.UI.DebugToggleTexts;
using LogicWorld.UI.HelpList;
using ThisOtherThing.UI.Shapes;
using TMPro;
using UnityEngine;

namespace CustomWirePlacer.Client.Windows
{
	public class CWPStatusOverlay : MonoBehaviour, ILocalizedObject
	{
		private static GameObject rootObject;
		private static RectTransform windowRect;

		private static CWPStatusOverlay instance;
		private static bool genericDirty = true;

		private const float h = 0.5f;

		public static void Init()
		{
			//Static initialization, create the game object, which this window is attached to.
			if(rootObject != null)
			{
				throw new Exception("Already initialized CWP-StatusDisplay");
			}
			rootObject = WindowHelper.makeOverlayCanvas("CWP: StatusDisplay root");

			//Add status Display:
			GameObject windowObject = WindowHelper.makeGameObject("CWP: StatusDisplay window");
			{
				RectTransform rectTransform = windowObject.AddComponent<RectTransform>();
				rectTransform.anchorMin = new Vector2(0, 1);
				rectTransform.anchorMax = new Vector2(0, 1);
				rectTransform.pivot = new Vector2(0, 1);
				rectTransform.sizeDelta = new Vector2(300, 200);
				windowRect = rectTransform;

				windowObject.AddComponent<CanvasRenderer>();

				windowObject.SetActive(true);
				windowObject.setParent(rootObject);
			}

			//Add background:
			{
				GameObject backgroundObject = WindowHelper.makeGameObject("CWP - Status background");
				RectTransform rectTransform = backgroundObject.AddComponent<RectTransform>();
				rectTransform.anchorMin = new Vector2(0, 0);
				rectTransform.anchorMax = new Vector2(1, 1);
				rectTransform.pivot = new Vector2(h, h);
				rectTransform.sizeDelta = new Vector2(0, 0);

				backgroundObject.AddComponent<CanvasRenderer>();

				Rectangle background = backgroundObject.AddComponent<Rectangle>();
				background.color = new Color(0f, 0f, 0f, 0.5f);

				backgroundObject.SetActive(true);
				backgroundObject.setParent(windowObject);
			}

			//Add foreground:
			{
				GameObject textObject = WindowHelper.makeGameObject("CWP - Status text");
				RectTransform rectTransform = textObject.AddComponent<RectTransform>();
				rectTransform.anchorMin = new Vector2(0, 1);
				rectTransform.anchorMax = new Vector2(0, 1);
				rectTransform.pivot = new Vector2(0, 1);
				rectTransform.sizeDelta = new Vector2(0, 0);
				textObject.AddComponent<CanvasRenderer>();

				textObject.AddComponent<TextMeshProUGUI>();

				CWPStatusOverlay window = textObject.AddComponent<CWPStatusOverlay>();
				window.Initialize();

				textObject.SetActive(true);
				textObject.setParent(windowObject);
			}
		}

		public static void setVisible(bool val)
		{
			rootObject.SetActive(val);
			instance.updatePosition(true);
		}

		private TextMeshProUGUI textMesh;

		public void Initialize()
		{
			instance = this;

			//Add a text-field:
			textMesh = gameObject.GetComponent<TextMeshProUGUI>();
			textMesh.fontSize = 40;
			textMesh.verticalAlignment = VerticalAlignmentOptions.Top;
			textMesh.horizontalAlignment = HorizontalAlignmentOptions.Left;
			textMesh.autoSizeTextContainer = true;
			textMesh.overflowMode = TextOverflowModes.Overflow;
			textMesh.enableWordWrapping = false;

			fieldRect = typeof(DebugToggleTextManager).GetField("PlaceholdersParent", BindingFlags.Instance | BindingFlags.NonPublic);
			if(fieldRect == null)
			{
				ModClass.logger.Error("Could not find field 'PlaceholdersParents' in class 'DebugToggleTextManager'. Cannot properly set status display depending on debug window state.");
			}

			TextLocalizer.Initialize(this);
		}

		private void setText(string text)
		{
			textMesh.text = text;
			windowRect.sizeDelta = new Vector2(textMesh.preferredWidth, textMesh.preferredHeight);
		}

		private FieldInfo fieldRect;
		private DebugToggleTextManager obj;
		private bool keyHelpOpen;
		private byte debugState;

		private void updatePosition(bool dirty = false)
		{
			bool keyHelpOpen = ToggleableSingletonMenu<HelpListMenu>.MenuIsVisible;
			if(keyHelpOpen != this.keyHelpOpen)
			{
				this.keyHelpOpen = keyHelpOpen;
				dirty = true;
			}

			byte debugState = (byte) ((ToggleableSingletonMenu<DebugFpsText>.MenuIsVisible ? 2 : 0) | (ToggleableSingletonMenu<DebugCoordinatesText>.MenuIsVisible ? 1 : 0));
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
					obj = FindObjectOfType<DebugToggleTextManager>();
					if(obj == null)
					{
						ModClass.logger.Error("Could not find instance of 'DebugToggleTextManager', cannot set status display at right position when debugging window.");
						fieldRect = null; //Disable.
						windowRect.anchoredPosition = new Vector2(0, 0);
						return;
					}
				}
				RectTransform rectTransform = (RectTransform) fieldRect.GetValue(obj);
				windowRect.anchoredPosition = new Vector2(0, -rectTransform.sizeDelta.y);
			}
			else
			{
				windowRect.anchoredPosition = new Vector2(0, 0);
			}
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
			CWPGroup firstGroup = CWP.CustomWirePlacer.getFirstGroup();
			CWPGroup secondGroup = CWP.CustomWirePlacer.getSecondGroup();
			CWPGroup currentGroup = CWP.CustomWirePlacer.getCurrentGroup();
			StringBuilder sb = new StringBuilder();
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
				if(firstGroup.isTwoDimensional())
				{
					sb.Append(TextLocalizer.LocalizedFormat("CWP.StatusOverlay.Dim2Pegs", 2, firstGroup.getPegCount(),
					                                        firstGroup.getFirstAxis().getPegCount(), firstGroup.getSecondAxis().getPegCount()));
				}
				else
				{
					sb.Append(TextLocalizer.LocalizedFormat("CWP.StatusOverlay.Dim1Pegs", 2, firstGroup.getPegCount()));
				}
				sb.Append('\n');
			}

			//Current axis details:
			CWPGroupAxis currentAxis = currentGroup.getCurrentAxis();
			int forward = currentAxis.forwards != null ? currentAxis.forwards.Count : 0;
			int backward = currentAxis.backwards != null ? currentAxis.backwards.Count : 0;
			if(forward != 0 || backward != 0)
			{
				sb.Append(TextLocalizer.LocalizedFormat("CWP.StatusOverlay.Expand", backward, forward)).Append('\n');
			}

			//Skipping:
			int amount = currentGroup.getCurrentAxis().skipNumber;
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
