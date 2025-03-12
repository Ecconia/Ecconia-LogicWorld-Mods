using System.Linq;
using LICC;
using LogicUI.MenuTypes.ConfigurableMenus;
using UnityEngine.SceneManagement;

namespace EccsGuiBuilder.Client
{
	public static class YPositionCommand
	{
		[Command(name: "EccsGuiBuilder.PrintYPosition")]
		private static void commandPrintYPosition()
		{
			foreach (var menu in SceneManager.GetActiveScene().GetRootGameObjects()
				         .Where(entry => entry.activeSelf)
				         .Select(entry => entry.GetComponent<ConfigurableMenu>())
				         .Where(entry => entry != null)
			)
			{
				var menuRect = menu.getMenuRectTransform();
				LConsole.WriteLine("- " + menu.name + ": Y = " + menuRect.anchoredPosition.y);
			}
		}
	}
}
