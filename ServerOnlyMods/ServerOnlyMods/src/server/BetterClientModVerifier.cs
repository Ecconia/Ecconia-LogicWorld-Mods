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
			List<MetaMod> missingMods = new List<MetaMod>();
			string[] clientMods = ctx.ApprovalPacket.ClientMods;
			foreach(var metaMod in ServerOnlyMods.getRequiredMods())
			{
				string modName = metaMod.Manifest.ID;
				if(!clientMods.Contains(modName))
				{
					missingMods.Add(metaMod);
				}
			}
			if(missingMods.Count != 0)
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
		
		private static string modToString(MetaMod mod)
		{
			return mod.Manifest.ID + ":" + mod.Manifest.Version;
		}
	}
}
