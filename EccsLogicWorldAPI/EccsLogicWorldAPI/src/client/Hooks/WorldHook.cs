using System;
using System.Linq;
using LogicWorld.SharedCode.WorldTypes;
using UnityEngine.SceneManagement;

namespace EccsLogicWorldAPI.Client.Hooks
{
	public static class WorldHook
	{
		/**
		 * Called, when the world (grassland / gridland / etc).
		 *
		 * Be careful: LogicWorld might not yet have called 'initialize' on everything in that scene.
		 */
		public static event Action<Scene> worldTypeLoading;
		/**
		 * Called, when the gameplay UI is being loaded - this happens after the world type is loaded.
		 *
		 * Be careful: LogicWorld might not yet have called 'initialize' on everything in that scene.
		 */
		public static event Action<Scene> worldUILoading;
		/**
		 * Called, when the gameplay scripts (barrier / player movement) are being loaded - this happens after the world type is loaded.
		 *
		 * Be careful: LogicWorld might not yet have called 'initialize' on everything in that scene.
		 */
		public static event Action<Scene> worldGameplayLoading;
		/**
		 * Called, after the world type and UI and scripts are being loaded.
		 *  The order in which UI and scripts are being loaded as scenes might change in future.
		 *  Hence this helper exists, so that only this mod and not your mod needs to update.
		 *
		 * Be careful: LogicWorld might not yet have called 'initialize' on everything in these scene.
		 */
		public static event Action worldLoading;
		
		/**
		 * The world and thus its helper scenes are being unloaded.
		 * Happens on error and when the player closes the world or game.
		 */
		public static event Action worldUnloading;
		
		public static bool isWorldSceneLoaded { private set; get; }
		public static bool isWorldUISceneLoaded { private set; get; }
		public static bool isWorldGameplaySceneLoaded { private set; get; }
		
		static WorldHook()
		{
			SceneManager.sceneLoaded += (scene, mode) =>
			{
				//LConsole.WriteLine("+SCENE " + scene.name + " " + mode);
				if(mode == LoadSceneMode.Single)
				{
					if(isWorldSceneName(scene.name))
					{
						isWorldSceneLoaded = true;
						worldTypeLoading?.Invoke(scene);
					}
					return;
				}
				if(!isWorldSceneLoaded)
				{
					return; //This scene is not important.
				}
				if("GameplayStuff".Equals(scene.name))
				{
					isWorldGameplaySceneLoaded = true;
					worldGameplayLoading?.Invoke(scene);
				}
				else if("UI_Gameplay".Equals(scene.name))
				{
					isWorldUISceneLoaded = true;
					worldUILoading?.Invoke(scene);
				}
				if(isWorldGameplaySceneLoaded && isWorldUISceneLoaded)
				{
					worldLoading?.Invoke();
				}
			};
			SceneManager.sceneUnloaded += scene =>
			{
				//LConsole.WriteLine("-SCENE " + scene.name);
				if(isWorldSceneName(scene.name))
				{
					isWorldSceneLoaded = false;
					isWorldUISceneLoaded = false;
					isWorldGameplaySceneLoaded = false;
					worldUnloading?.Invoke();
				}
			};
		}
		
		private static bool isWorldSceneName(string sceneName)
		{
			return WorldTypesManager.WorldTypesList.Any(worldType => worldType.Data.SceneName.Equals(sceneName));
		}
	}
}
