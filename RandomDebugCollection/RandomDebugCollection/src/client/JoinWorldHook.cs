using System.Reflection;
using HarmonyLib;
using LogicWorld;

namespace RandomDebugCollection.Client
{
	public static class JoinWorldHook
	{
		public static void init()
		{
			var meth = typeof(GameStarter).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
			var handleMeth = typeof(JoinWorldHook).GetMethod("handle", BindingFlags.Public | BindingFlags.Static);
			new Harmony("Launch arg hook").Patch(meth, new HarmonyMethod(handleMeth));
		}

		public static bool handle()
		{
			string[] args = System.Environment.GetCommandLineArgs();
			for(int i = 0; i < args.Length; i++)
			{
				if(args[i].ToLower() == "-loadworld" && args.Length > i)
				{
					string input = args[i + 1];
					SceneAndNetworkManager.ConnectToIntegratedServer(input, false);
					RandomDebugCollection.logger.Info("Stopping normal execution, loading into save! Name: '" + input + "'");
					return false;
				}
			}
			return true;
		}
	}
}
