using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using HarmonyLib;
using LogicWorld;

namespace RandomDebugCollection.Client
{
	public static class JoinWorldHook
	{
		public static void init()
		{
			var meth = Methods.getPrivate(typeof(GameStarter), "Start");
			var handleMeth = Methods.getPublicStatic(typeof(JoinWorldHook), nameof(handle));
			new Harmony("Launch arg hook").Patch(meth, new HarmonyMethod(handleMeth));
		}
		
		public static bool handle()
		{
			var args = Environment.GetCommandLineArgs();
			for(var i = 0; i < args.Length; i++)
			{
				if(args[i].ToLower() == "-loadworld" && args.Length > i)
				{
					var input = args[i + 1];
					SceneAndNetworkManager.ConnectToIntegratedServer(input, false);
					RandomDebugCollection.logger.Info("Stopping normal execution, loading into save! Name: '" + input + "'");
					return false;
				}
			}
			return true;
		}
	}
}
