using System;
using System.Text;
using CustomWirePlacer.Client.CWP;
using TMPro;
using UnityEngine;

namespace CustomWirePlacer.Client.Windows
{
	public class CWPStatusDisplay : MonoBehaviour
	{
		private static GameObject rootObject;

		public static void Init()
		{
			//Static initialization, create the game object, which this window is attached to.
			if(rootObject != null)
			{
				throw new Exception("Already initialized CWP-StatusDisplay");
			}
			rootObject = new GameObject("CWP - Status root");
			DontDestroyOnLoad(rootObject);
			rootObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
			rootObject.SetActive(false);

			//Add background:
			{
				GameObject backgroundObject = new GameObject("CWP - Status background");
				DontDestroyOnLoad(backgroundObject);
				BagkrountDezd background = backgroundObject.AddComponent<BagkrountDezd>();
				background.color = new Color(0f, 0f, 0f, 0.5f);
				RectTransform transform = background.rectTransform;
				transform.sizeDelta = new Vector2(200f, 200f);
				transform.position = new Vector3(5, -5, 0);
				transform.anchorMin = new Vector2(0, 1);
				transform.anchorMax = new Vector2(0, 1);
				transform.pivot = new Vector2(0, 1);
				backgroundObject.transform.SetParent(rootObject.transform, false);
			}

			//Add foreground:
			{
				GameObject textObject = new GameObject("CWP - Status text");
				DontDestroyOnLoad(textObject);
				CWPStatusDisplay window = textObject.AddComponent<CWPStatusDisplay>();
				window.Initialize();
				textObject.transform.SetParent(rootObject.transform, false);
			}
		}

		public static void setVisible(bool val)
		{
			rootObject.SetActive(val);
		}

		private TextMeshProUGUI textMesh;

		public void Initialize()
		{
			//Add a text-field:
			textMesh = gameObject.AddComponent<TextMeshProUGUI>();
			textMesh.fontSize = 30;
			textMesh.verticalAlignment = VerticalAlignmentOptions.Top;
			textMesh.horizontalAlignment = HorizontalAlignmentOptions.Left;
			
			RectTransform textTransform = textMesh.rectTransform;
			textTransform.position = new Vector3(10, -10, 0);
			textTransform.sizeDelta = new Vector2(200, 200);
			textTransform.anchorMin = new Vector2(0, 1);
			textTransform.anchorMax = new Vector2(0, 1);
			textTransform.pivot = new Vector2(0, 1);
			constructText();
		}

		private int groupAmount = 1;
		private bool flipped;
		private int amountOfPegs = 0;

		private void Update()
		{
			bool update = false;
			if(flipped != CWPSettings.flipping)
			{
				flipped = CWPSettings.flipping;
				update = true;
			}
			
			// IReadOnlyCollection<PegAddress> pegs = CustomWirePlacer.pegs;
			// if(pegs == null && amountOfPegs != 0)
			// {
			// 	amountOfPegs = 0;
			// 	update = true;
			// }
			// else if(pegs != null && CustomWirePlacer.pegs.Count != amountOfPegs)
			// {
			// 	amountOfPegs = pegs.Count;
			// 	update = true;
			// }
			if(update)
			{
				constructText();
			}
		}

		private void constructText()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Groups: <color=yellow>").Append(groupAmount).Append("</color>\n");
			sb.Append("Count: <color=yellow>").Append(amountOfPegs).Append("</color>\n");
			sb.Append("Flipped: <color=yellow>").Append(flipped).Append("</color>");
			textMesh.text = sb.ToString();
		}
	}
}
