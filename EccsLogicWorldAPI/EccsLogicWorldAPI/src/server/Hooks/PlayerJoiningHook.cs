using System;
using System.Collections.Generic;
using EccsLogicWorldAPI.Shared;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI.Data;
using LogicAPI.Networking;
using LogicWorld.Server;

namespace EccsLogicWorldAPI.Server.Hooks
{
	public static class PlayerJoiningHook
	{
		private static List<PlayerJoiningCallback> callbacks;
		
		private static void init()
		{
			if(callbacks != null)
			{
				return;
			}
			try
			{
				HarmonyAtRuntime.init();
			}
			catch(Exception e)
			{
				throw new Exception("[EccsLwApi] HarmonyForLogicWorld must be installed to use player join callbacks.", e);
			}
			var harmony = HarmonyAtRuntime.getHarmonyInstance("PlayerJoiningHook");
			var patch = Methods.getPublicStatic(typeof(Patches), nameof(Patches.Patch));
			var method = Methods.getPublic(typeof(PlayerManager), nameof(PlayerManager.PlayerJoinedWorld));
			HarmonyAtRuntime.patch(harmony, method, patch);
			
			callbacks = new List<PlayerJoiningCallback>();
		}
		
		public static void registerCallback(PlayerJoiningCallback callback)
		{
			init();
			callbacks.Add(callback);
		}
		
		public interface PlayerJoiningCallback
		{
			void playerIsJoining(Connection connection, PlayerData playerData);
		}
		
		private static class Patches
		{
			public static void Patch(Connection connection, PlayerData playerData)
			{
				foreach(PlayerJoiningCallback callback in callbacks)
				{
					callback.playerIsJoining(connection, playerData);
				}
			}
		}
	}
}
