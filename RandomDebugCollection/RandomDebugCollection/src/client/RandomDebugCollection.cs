using LogicAPI.Client;

using RandomDebugCollection.Commands;

namespace RandomDebugCollection
{
	public class RandomDebugCollection : ClientMod
	{
		protected override void Initialize()
		{
			ClearSubassemblies.Initialize(Logger);
			ClearHistory.Initialize(Logger);
			StacktraceToLog.Initialize(Logger);
		}
	}
}
