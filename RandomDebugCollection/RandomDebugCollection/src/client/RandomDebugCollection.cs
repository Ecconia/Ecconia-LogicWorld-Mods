using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using HarmonyLib;
using LogicAPI.Client;
using LogicLog;
using RandomDebugCollection.Client.Commands;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
			PrintCompilationErrors.Initialize(Logger);
			JoinWorldHook.init();

			UnityEngine.Application.logMessageReceived += ApplicationOnlogMessageReceived;
			System.AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnFirstChanceException;
		}

        private void ApplicationOnlogMessageReceived(string condition, string stacktrace, LogType type)
        {
            LogLevel ll = LogLevel.Information;
            ll = Enum.TryParse<LogLevel>(type.ToString(), true, out var llt) ? llt : ll;
			logger.Log($"{condition}\n{stacktrace}",ll);
        }

        private void CurrentDomainOnFirstChanceException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Error("Captured Exception:\n" + ((Exception)e.ExceptionObject).AsLogMessage());
        }
    }
}
