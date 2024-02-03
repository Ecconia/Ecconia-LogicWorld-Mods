using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LICC;
using LogicWorld.BuildingManagement;

namespace RandomDebugCollection.Client.Commands
{
	public static class ClearHistory
	{
		private static readonly Func<int> getHistorySize;
		private static readonly Action resetHistory;
		
		static ClearHistory()
		{
			getHistorySize = Delegator.createStaticFieldGetter<int>(Fields.getPrivateStatic(typeof(UndoManager), "IndexOfNextNewBuildAction"));
			resetHistory = Delegator.createStaticMethodCall(Methods.getPrivateStatic(typeof(UndoManager), "Reset"));
		}
		
		[Command("ClearHistory", Description = "Clears the edit history.")]
		public static void clearHistory()
		{
			var value = getHistorySize();
			resetHistory();
			LConsole.WriteLine("Cleared {0} history entries.", value);
		}
	}
}
