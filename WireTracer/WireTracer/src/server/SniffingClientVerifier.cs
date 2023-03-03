using LogicAPI.Server.Networking.ClientVerification;

namespace WireTracer.Server
{
	public class SniffingClientVerifier : IClientVerifier
	{
		private readonly WireTracerServer wireTracerServer;

		public SniffingClientVerifier(WireTracerServer wireTracerServer)
		{
			this.wireTracerServer = wireTracerServer;
		}

		public void Verify(VerificationContext ctx)
		{
			foreach(var modID in ctx.ApprovalPacket.ClientMods)
			{
				if(modID.Equals("WireTracer"))
				{
					//Found a client that has WireTracer!
					wireTracerServer.playerHasMod(ctx.PlayerID.Name, true);
					return;
				}
			}
			//Client does not have WireTracer!
			wireTracerServer.playerHasMod(ctx.PlayerID.Name, false);
		}
	}
}
