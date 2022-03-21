using System;
using System.Collections.Generic;
using LogicLog;
using HarmonyLib;
using LogicAPI.Data;
using LogicAPI.Networking;
using LogicWorld.Server;

namespace ServerModdingTools.Server
{
	public static class PlayerJoiningHook
	{
		private static List<PlayerJoiningCallback> callbacks;

		public static void inject(ILogicLogger logger)
		{
			new Harmony("PlayerJoiningHook").PatchAll();
			logger.Info("Hooked into player-join process.");

			callbacks = new List<PlayerJoiningCallback>();
		}

		public static bool isAvailable()
		{
			return callbacks != null;
		}

		public static void registerCallback(PlayerJoiningCallback callback)
		{
			if(!isAvailable())
			{
				throw new Exception("Attempted to register a callback, but the callback hook was not registered.");
			}
			callbacks.Add(callback);
		}

		[HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.PlayerJoinedWorld))]
		private static class Patch
		{
			public static void Prefix(Connection connection, PlayerData playerData)
			{
				foreach(PlayerJoiningCallback callback in callbacks)
				{
					callback.playerIsJoining(connection, playerData);
				}
			}
		}
	}
}
