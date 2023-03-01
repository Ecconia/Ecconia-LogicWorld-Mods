
using LogicAPI;
using LogicAPI.Client;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EcconiaCPUServerComponents.Client
{
	public class ModClass : ClientMod
	{
        ModEditComponentMenu
		public static AssetBundle modAssets { get; private set; }
        public static AssetBundle uiAssets { get; private set; }
        public static IDispatcher dispatcher { get; private set; }
        protected override void Initialize()
        {
            dispatcher = Dispatcher;
            //Logger.Info( string.Join(":",Enumerable.Range(0,32).Select(LayerMask.LayerToName)));
            modAssets = Assets.LoadAssetBundle("display");
            //No initialization code for this mod (yet).
            uiAssets = Assets.LoadAssetBundle("uielements");

            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        }

        private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if(arg0.buildIndex != 6) return;
            Logger.Info("Loading Ram Menu!");

            var menuInst = uiAssets.InstantiateAsset<GameObject>("MenuBase");
            var menu = menuInst.AddComponent<Ram256b8DEditMenu>();
            SceneManager.MoveGameObjectToScene(menuInst,arg0);
            Logger.Info("Call Initialize On Ram Menu");
            Helper.InitializeRecursive(menuInst);
        }
    }
}
