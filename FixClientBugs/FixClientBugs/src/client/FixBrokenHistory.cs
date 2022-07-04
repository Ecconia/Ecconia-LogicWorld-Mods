using System.Collections;
using System.Reflection;
using HarmonyLib;
using LogicWorld.BuildingManagement;

namespace FixClientBugs.Client
{
	public static class FixBrokenHistory
	{
		private static FieldInfo fieldHistory;
		private static FieldInfo fieldIndex;
		private static IList list;

		public static void init(Harmony harmony)
		{
			fieldHistory = typeof(UndoManager).GetField("UndoHistory", BindingFlags.Static | BindingFlags.NonPublic);
			fieldIndex = typeof(UndoManager).GetField("IndexOfNextNewBuildAction", BindingFlags.Static | BindingFlags.NonPublic);
			if(fieldHistory == null || fieldIndex == null)
			{
				ModClass.logger.Error("Could not find UndoManager fields, cannot stop a history bug from occuring. Use UNDO with care!");
				return;
			}

			list = (IList) fieldHistory.GetValue(null);
			if(list == null)
			{
				ModClass.logger.Error("Could not load history list, cannot stop a history bug from occuring. Use UNDO with care!");
				return;
			}

			var methodStartMultiUndo = typeof(UndoManager).GetMethod("MarkStartOfMultiUndo", BindingFlags.Static | BindingFlags.NonPublic);
			if(methodStartMultiUndo == null)
			{
				ModClass.logger.Error("Could not find method 'MarkStartOfMultiUndo' in 'UndoManager', cannot stop a history bug from occuring. Use UNDO with care!");
				return;
			}
			var patch = typeof(FixBrokenHistory).GetMethod(nameof(fixHistory), BindingFlags.Public | BindingFlags.Static);
			harmony.Patch(methodStartMultiUndo, new HarmonyMethod(patch));
		}

		public static void fixHistory()
		{
			int index = (int) fieldIndex.GetValue(null);
			if(list.Count > index)
			{
				while(list.Count > index)
				{
					list.RemoveAt(index);
				}
			}
		}
	}
}
