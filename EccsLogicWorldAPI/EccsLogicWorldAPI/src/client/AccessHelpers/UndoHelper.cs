using System;
using System.Collections.Generic;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI.Data.BuildingRequests;
using LogicWorld.BuildingManagement;

namespace EccsLogicWorldAPI.Client.AccessHelpers
{
	public static class UndoHistory
	{
		private static readonly Action<BuildRequestReceipt> handleReceipt;
		
		static UndoHistory()
		{
			handleReceipt = Delegator.createStaticMethodCall<BuildRequestReceipt>(Methods.getPrivateStatic(typeof(UndoManager), "ReceiptReceivedForUndoableBuildAction"));
		}
		
		public static bool addToUndoList(IEnumerable<BuildRequest> undoRequests)
		{
			handleReceipt(new BuildRequestReceipt()
			{
				ActionSuccessfullyApplied = true,
				RequestsToUndo = undoRequests,
			});
			return true;
		}
	}
}
