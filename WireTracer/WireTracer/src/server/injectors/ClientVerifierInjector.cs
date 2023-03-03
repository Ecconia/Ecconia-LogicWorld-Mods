using System.Collections.Generic;
using System.Reflection;
using LogicAPI.Server.Networking.ClientVerification;
using LogicLog;
using LogicWorld.Server;

namespace WireTracer.Server.Injectors
{
	public static class ClientVerifierInjector
	{
		public static bool injectNewType(ILogicLogger logger, IClientVerifier clientVerifier)
		{
			INetworkManager iNetworkManager = Program.Get<INetworkManager>();
			if(iNetworkManager == null)
			{
				logger.Error("Could not get service INetworkManager.");
				return false;
			}
			if(iNetworkManager.GetType() != typeof(NetworkManager))
			{
				logger.Error("Service INetworkManager is not a NetworkManager class.");
				return false;
			}

			var field = typeof(NetworkManager).GetField("ClientVerifiers", BindingFlags.NonPublic | BindingFlags.Instance);
			if(field == null)
			{
				logger.Error("Could not find verifier list in NetworkManager.");
				return false;
			}
			var oldVerifierList = (IEnumerable<IClientVerifier>) field.GetValue(iNetworkManager);
			if(oldVerifierList == null)
			{
				logger.Error("Issue getting the IClientVerifier List from field.");
				return false;
			}

			var newVerifierList = new List<IClientVerifier>(oldVerifierList);
			newVerifierList.Add(clientVerifier);
			field.SetValue(iNetworkManager, newVerifierList);
			logger.Debug("Added 'ModListSniffingVerifier'.");
			return true;
		}
	}
}
