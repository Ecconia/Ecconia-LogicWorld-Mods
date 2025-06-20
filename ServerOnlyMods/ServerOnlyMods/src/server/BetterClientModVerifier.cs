using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicAPI;
using LogicAPI.Server.Networking.ClientVerification;

namespace ServerOnlyMods.Server
{
	public class BetterClientModVerifier : IClientVerifier
	{
		public void Verify(VerificationContext ctx)
		{
			var missingMods = new List<MetaMod>();
			var clientMods = ctx.ApprovalPacket.ClientMods;
			foreach(var metaMod in ServerOnlyMods.getRequiredMods())
			{
				var modName = metaMod.Manifest.ID;
				if(!clientMods.Select(element => element.modId).Contains(modName))
				{
					missingMods.Add(metaMod);
				}
			}
			if(missingMods.Count != 0)
			{
				var builder = new StringBuilder();
				builder.Append("You are missing the following mods:\n");
				builder.Append(modToString(missingMods[0]));
				for(var i = 1; i < missingMods.Count; i++)
				{
					builder.Append(", ");
					builder.Append(modToString(missingMods[i]));
				}
				ctx.DenyConnection(builder.ToString());
			}
		}
		
		private static string modToString(MetaMod mod)
		{
			return mod.Manifest.ID + ":" + mod.Manifest.Version;
		}
	}
}
