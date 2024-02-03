using System.Collections.Generic;
using EccsLogicWorldAPI.Server.Injectors;
using LogicAPI;
using LogicAPI.Server;
using LogicWorld.Server.Networking.ClientVerification;
using LogicWorld.SharedCode.Modding;

namespace ServerOnlyMods.Server
{
	public class ServerOnlyMods : ServerMod
	{
		//You have a reason to access this with reflection?
		// Maybe rather contact the developer Ecconia, and tell him what this mod is lacking or doing wrong...
		private static List<MetaMod> clientRequiredMods;
		
		protected override void Initialize()
		{
			RawJoinVerifierInjector.replaceVerifier(typeof(ModsVerifier), new BetterClientModVerifier());
		}
		
		public static List<MetaMod> getRequiredMods()
		{
			if(clientRequiredMods == null)
			{
				List<MetaMod> collectedMods = new List<MetaMod>();
				foreach(var modMeta in ModLoader.LoadedMods.Values)
				{
					if(modMeta.Files.Exists("OnlyServerMod"))
					{
						//Skip all mods which are server sided only. (They may be on the client anyway).
						continue;
					}
					collectedMods.Add(modMeta);
				}
				clientRequiredMods = collectedMods; //Replace at once, to not run into any async issues (if any).
			}
			return clientRequiredMods;
		}
	}
}
