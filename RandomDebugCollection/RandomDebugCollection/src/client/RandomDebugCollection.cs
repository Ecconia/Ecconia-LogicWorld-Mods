using LogicAPI.Client;
using LogicLog;
using RandomDebugCollection.Client.Commands;

namespace RandomDebugCollection.Client
{
	public class RandomDebugCollection : ClientMod
	{
		public static ILogicLogger logger;

		protected override void Initialize()
		{
			logger = Logger;
			ClearSubassemblies.Initialize(Logger);
			ClearHistory.Initialize(Logger);
			StacktraceToLog.Initialize(Logger);
			JoinWorldHook.init();
		}
	}
}
