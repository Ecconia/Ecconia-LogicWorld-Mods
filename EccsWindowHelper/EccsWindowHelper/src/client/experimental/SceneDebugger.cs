using LICC;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EccsWindowHelper.Client.Experimental
{
	public class SceneDebugger
	{
		[Command]
		public static void yili()
		{
			int count = SceneManager.sceneCountInBuildSettings;
			LConsole.WriteLine("There are " + count + " scenes.");
			for(int i = 0; i < count; i++)
			{
				Scene scene = SceneManager.GetSceneByBuildIndex(i);
				LConsole.WriteLine(i + ": <color=yellow>" + scene.name + "</color> " + (scene.isLoaded ? "(<color=green>loaded</color>)" : "(<color=red>unloaded</color>)"));
				LConsole.WriteLine(" //" + SceneUtility.GetScenePathByBuildIndex(i));
				debugScene(scene);
			}
		}

		private static void debugScene(Scene scene)
		{
			if(scene.isLoaded)
			{
				LConsole.WriteLine(" -> <color=#0ff>" + scene.GetRootGameObjects().Length + "</color>");
				foreach(GameObject gameObject in scene.GetRootGameObjects())
				{
					LConsole.WriteLine("   - " + gameObject.name + " " + (scene.isLoaded ? "(<color=green>active</color>)" : "(<color=red>inactive</color>)"));
				}
			}
		}
	}
}
