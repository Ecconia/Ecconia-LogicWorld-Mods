using System;
using System.Text.RegularExpressions;
using HarmonyLib;
using JimmysUnityUtilities;
using LogicLog;
using LogicWorld;
using LogicWorld.UI;
using UnityEngine;

namespace RandomDebugCollection.Client
{
	public static class StacktraceToLog
	{
		public static void Initialize(ILogicLogger logger)
		{
			new Harmony("RandomDebugCollection.StacktraceToLog").PatchAll();
			Application.logMessageReceivedThreaded += (condition, trace, type) =>
			{
				if(type != LogType.Exception)
				{
					return; //Only exceptions are interesting.
				}
				if(trace.Contains("Rethrow as ExWrapper: Exception of type 'RandomDebugCollection.Client.ExWrapper' was thrown."))
				{
					return; //This exception was already printed to console/logs via the error-display-screen.
				}
				//"Condition" contains the class name with colon and message.
				trace = Regex.Replace(trace, " \\(at <[0-9a-f]+>:0\\)", "");
				logger.Error("Captured Unity forwarded exception:\n" + condition + "\n" + trace);
			};
		}
	}
	
	[HarmonyPatch(typeof(SceneAndNetworkManager), nameof(SceneAndNetworkManager.TriggerErrorScreen), new Type[] {typeof(Exception)})]
	public static class Patch
	{
		public static bool Prefix(Exception exception)
		{
			var stacktrace = exception.ToString();
			stacktrace = Regex.Replace(stacktrace, " \\[0x[0-9a-f]+\\] in <[0-9a-f]+>:0 ", "");
			// logger.Error("Captured exception:\n" + stacktrace); //Triggering the error screen will already print a message to log - no containing the full stacktrace.
			//Wrap the exception, so that it can be filtered later on again, and not double catch it in console:
			Dispatcher.InvokeAsync(() => Debug.LogException(new ExWrapper(exception))); //Do not send the stacktrace to unity logs anymore! Not needed - honestly - its in the logs with showing on the error screen.
			SceneAndNetworkManager.TriggerErrorScreen(stacktrace);
			//Update the error message to some styled version:
			var lines = stacktrace.Split(new char[] {'\n'}, 2, StringSplitOptions.None);
			var formattedStacktrace = "<size=50%>" + lines[0] + "</size>\n<size=30%>" + lines[1].Split('\n').Join(null, "</size>\n<size=30%>") + "</size>";
			//Pray that the dispatcher will run later than the original error message thingy. Else race condition of who posts the content...
			Dispatcher.Invoke(() => ErrorScreen.DisplayError(formattedStacktrace, true)); //The only time the main menu button is not shown, is when this mod is not even loaded.
			return false; //Manually invoke what that function does, to provide a better error message.
		}
	}
	
	//Wrapper class, used to wrap exceptions that are sent to Unity Log, so that the Unity Log handler recognizes it and won't rethrow it.
	// One could skip sending this thing to log at all, as the stacktrace is logged and the game is not running with Unity.
	// But well, although this modifies the exception, it will make it compatible with Unity.
	public class ExWrapper : Exception
	{
		public ExWrapper(Exception parent) : base(null, parent)
		{
		}
	}
}
