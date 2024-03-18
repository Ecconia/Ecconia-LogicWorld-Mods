using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using HarmonyLib;
using LogicWorld;
using RandomDebugCollection.Shared;

namespace RandomDebugCollection.Client
{
	public static class StartupArgumentsHook
	{
		public static void init()
		{
			var meth = Methods.getPrivate(typeof(GameStarter), "Start");
			var handleMeth = Methods.getPublicStatic(typeof(StartupArgumentsHook), nameof(handle));
			new Harmony("Launch arg hook").Patch(meth, new HarmonyMethod(handleMeth));
		}
		
		public static bool handle()
		{
			var args = Environment.GetCommandLineArgs();
			for(var i = 0; i < args.Length; i++)
			{
				// Version must be first argument, as I am too lazy to write decent code for this.
				if(args[i].ToLower() == "-version" && args.Length > i)
				{
					var input = args[++i];
					VersionOverride.overrideVersion(input);
				}
			}
			return true;
		}
	}
}
