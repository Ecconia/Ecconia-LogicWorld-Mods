using UnityEngine;
using UnityEngine.UI;

namespace EccsGuiBuilder.Client.Layouts.Elements
{
	public class IgnoreLayout : MonoBehaviour, ILayoutIgnorer
	{
		public bool ignoreLayout => true;
	}
}
