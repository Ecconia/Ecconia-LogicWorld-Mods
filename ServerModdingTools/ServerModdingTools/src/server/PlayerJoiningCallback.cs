using LogicAPI.Data;
using LogicAPI.Networking;

namespace ServerModdingTools.Server
{
	public interface PlayerJoiningCallback
	{
		void playerIsJoining(Connection connection, PlayerData playerData);
	}
}
