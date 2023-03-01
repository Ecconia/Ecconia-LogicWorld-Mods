using System;
using LogicUI;
using LogicLog;
using LogicWorld.References;
using LogicWorld.UnityHacksAndExtensions;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using LogicWorld;

namespace EcconiaCPUServerComponents.Client
{
	public static class Helper
	{
        private static ILogicLogger Logger = LogicLogger.For("Initialize Helper");

        public static void InitializeRecursive(GameObject gameObject)
        {
            bool anyErrorsHaveOcurredYet = false;

            IInitializable[] components = gameObject.GetComponents<IInitializable>();
            foreach (IInitializable initializable in components)
            {
                try
                {
                    Logger.Trace("Initializing object of type {0}", initializable.GetType().FullName);
                    initializable.Initialize();
                    Logger.Trace("Finished initializing object without issues");
                }
                catch (Exception exception)
                {
                    Debug.LogError($"Initializing type {initializable.GetType()} threw an exception!");
                    Debug.LogException(exception);
                    SceneAndNetworkManager.TriggerErrorScreen(exception);
                    anyErrorsHaveOcurredYet = true;
                }
            }

            if(anyErrorsHaveOcurredYet) return;

            foreach (Transform transform in gameObject.transform)
            {
                InitializeRecursive(transform.gameObject);
            }
        }


		public static (GameObject, TextMeshPro) textObjectMono(string name)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.SetActive(false); //Set it to inactive, so that TMP won't corrupt the default material
			TextMeshPro textRenderer = gameObject.AddComponent<TextMeshPro>(); //Add TMP to the GameObject
			textRenderer.fontSharedMaterial = Object.Instantiate(Materials.NotoSansMono_WorldSpace); //Set its material to a copy of what LW uses
			textRenderer.font = Object.Instantiate(Fonts.NotoMono); //Also set the font to a copy of what LW uses
			gameObject.SetActive(true); //Set to active, as the critical part is over
			return (gameObject, textRenderer);
		}
	}
}
