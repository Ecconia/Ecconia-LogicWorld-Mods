using System;
using LICC;

namespace RandomDebugCollection.Client.Commands
{
	public static class CollectGarbage
	{
		[Command("CollectGarbage", Description = "Runs garbage collection.")]
		private static void collectGarbage()
		{
			LConsole.WriteLine("Running garbage collection...");
			GC.Collect();
			GC.WaitForPendingFinalizers();
			LConsole.WriteLine("Done.");
		}
	}
}
