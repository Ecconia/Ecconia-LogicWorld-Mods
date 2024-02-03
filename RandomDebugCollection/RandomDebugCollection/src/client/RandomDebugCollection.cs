using LogicAPI.Client;
using LogicLog;

namespace RandomDebugCollection.Client
{
	public class RandomDebugCollection : ClientMod
	{
		public static ILogicLogger logger;
		
		protected override void Initialize()
		{
			logger = Logger;
			StacktraceToLog.Initialize(Logger);
			JoinWorldHook.init();
		}
	}
}
