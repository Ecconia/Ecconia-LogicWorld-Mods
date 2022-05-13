using LogicAPI.Client;

using RandomDebugCollection.Client.Commands;

namespace RandomDebugCollection.Client
{
	public class RandomDebugCollection : ClientMod
	{
		protected override void Initialize()
		{
			ClearSubassemblies.Initialize(Logger);
			ClearHistory.Initialize(Logger);
			StacktraceToLog.Initialize(Logger);
			PrintCompilationErrors.Initialize(Logger);
		}
	}
}
