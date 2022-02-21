//Needed for reflection classes:
using System.Reflection;

//Needed for 'Command', 'LConsole':
using LICC;
//Needed for 'ILogicLogger':
using LogicLog;

//Needed for 'UndoManager':
using LogicWorld.BuildingManagement;

namespace RandomDebugCollection.Commands
{
	public static class ClearHistory
	{
		private static FieldInfo fieldUndoHistorySize;
		private static MethodInfo methodReset;

		public static void Initialize(ILogicLogger logger)
		{
			var type = typeof(UndoManager);
			var field = type.GetField("IndexOfNextNewBuildAction", BindingFlags.NonPublic | BindingFlags.Static);
			if(field == null)
			{
				logger.Error("[Command: ClearHistory] Could not find field 'IndexOfNextNewBuildAction', this command will not work.");
				return;
			}
			var method = type.GetMethod("Reset", BindingFlags.NonPublic | BindingFlags.Static);
			if(method == null)
			{
				logger.Error("[Command: ClearHistory] Could not find method 'Reset', this command will not work.");
				return;
			}
			//Do not set anything, if either could be null.
			fieldUndoHistorySize = field;
			methodReset = method;
		}

		private static bool isPrimed()
		{
			return fieldUndoHistorySize != null;
		}

		private static int getHistorySize()
		{
			return (int) fieldUndoHistorySize.GetValue(null);
		}

		private static void reset()
		{
			methodReset.Invoke(null, null);
		}

		[Command("ClearHistory", Description = "Clears the edit history.")]
		private static void clearHistory()
		{
			if(!isPrimed())
			{
				LConsole.WriteLine("Error: Cloud not find field 'SubassemblyCache' or method 'Reset'. Command aborts.", CColor.Red);
				return;
			}
			var value = getHistorySize();
			reset();
			LConsole.WriteLine("Cleared {0} history entries.", value);
		}
	}
}
