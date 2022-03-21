using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LogicAPI;
using LogicAPI.Server.Networking.ClientVerification;
using LogicLog;
using LogicWorld.Server;
using LogicWorld.Server.Networking.ClientVerification;
using LogicWorld.SharedCode.Modding;

namespace ServerOnlyMods.Server
{
	public class BetterClientModVerifier : IClientVerifier
	{
		public void Verify(VerificationContext ctx)
		{
			List<MetaMod> missingMods = new List<MetaMod>();
			string[] clientMods = ctx.ApprovalPacket.ClientMods;
			foreach(var entry in ModLoader.LoadedMods)
			{
				MetaMod modMeta = entry.Value;
				if(modMeta.ModInstance is IServerSideOnlyMod)
				{
					//Skip all mods which are server sided only. (They may be on the client anyway).
					continue;
				}
				string modName = entry.Key;
				if(!clientMods.Contains(modName))
				{
					missingMods.Add(modMeta);
				}
			}
			if(missingMods.Any())
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("You are missing the following mods:\n");
				builder.Append(modToString(missingMods[0]));
				for(int i = 1; i < missingMods.Count; i++)
				{
					builder.Append(", ");
					builder.Append(modToString(missingMods[i]));
				}
				ctx.DenyConnection(builder.ToString());
			}
		}

		private string modToString(MetaMod mod)
		{
			return mod.Manifest.ID + ":" + mod.Manifest.Version;
		}

		public static void inject(ILogicLogger logger)
		{
			INetworkManager iNetworkManager = Program.Get<INetworkManager>();
			if(iNetworkManager == null)
			{
				logger.Error("Could not get service INetworkManager.");
				return;
			}
			if(iNetworkManager.GetType() != typeof(NetworkManager))
			{
				logger.Error("Service INetworkManager is not a NetworkManager class.");
				return;
			}

			var field = typeof(NetworkManager).GetField("ClientVerifiers", BindingFlags.NonPublic | BindingFlags.Instance);
			if(field == null)
			{
				logger.Error("Could not find verified list in NetworkManager.");
				return;
			}
			IEnumerable<IClientVerifier> oldVerifierList = (IEnumerable<IClientVerifier>) field.GetValue(iNetworkManager);
			if(oldVerifierList == null)
			{
				logger.Error("Issue getting the IClientVerifier List from field.");
				return;
			}
			bool injected = false;
			List<IClientVerifier> newVerifierList = new List<IClientVerifier>();
			foreach(IClientVerifier oldVerifier in oldVerifierList)
			{
				if(oldVerifier.GetType() == typeof(ModsVerifier))
				{
					if(injected)
					{
						logger.Warn("Already replaced ModsVerifier, why is it a thing twice?");
						continue;
					}
					//Add new version instead:
					newVerifierList.Add(new BetterClientModVerifier());
					injected = true;
				}
				else
				{
					newVerifierList.Add(oldVerifier);
				}
			}
			if(!injected)
			{
				logger.Error("Did not find ModsVerifier, so did not replace it. Clients might not be able to join anymore. (Due to 'missing mods').");
				return;
			}
			field.SetValue(iNetworkManager, newVerifierList);
			logger.Info("Replaced 'ModVerifier'.");
		}
	}
}
