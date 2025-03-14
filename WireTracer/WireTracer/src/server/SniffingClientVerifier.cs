using System.Linq;
using LogicAPI.Server.Networking.ClientVerification;

namespace WireTracer.Server
{
	public class SniffingClientVerifier : IClientVerifier
	{
		private readonly WireTracerServer wireTracerServer;
		private readonly string serverModID;
		
		public SniffingClientVerifier(WireTracerServer wireTracerServer, string serverModID)
		{
			this.wireTracerServer = wireTracerServer;
			this.serverModID = serverModID;
		}
		
		public void Verify(VerificationContext ctx)
		{
			// Check if the client has this mod installed and remember it:
			wireTracerServer.playerHasMod(
				ctx.PlayerID.Name,
				ctx.ApprovalPacket.ClientMods.Contains(serverModID)
			);
		}
	}
}
