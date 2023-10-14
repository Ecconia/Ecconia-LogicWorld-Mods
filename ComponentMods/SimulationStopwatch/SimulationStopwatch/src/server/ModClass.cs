using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using EccsLogicWorldAPI.Server;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI.Server;
using LogicWorld.Server;

namespace SimulationStopwatch.Server
{
	public class ModClass : ServerMod
	{
		public static readonly double FACTOR_TIME_TO_MILLIS = (10000000.0 / Stopwatch.Frequency) / 10000L;
		
		protected override void Initialize()
		{
			var server = ServiceGetter.getService<IServer>();
			var field = Fields.getPrivate(server, "Tickers");
			var tickers = Types.checkType<IReadOnlyList<IServerTicker>>(Fields.getNonNull(field, server));
			
			var newList = new List<IServerTicker>();
			newList.Add(new CustomTicker(startServerTick));
			bool foundSimulation = false;
			foreach(var ticker in tickers)
			{
				if("SimulationManager".Equals(ticker.GetType().Name))
				{
					if(foundSimulation)
					{
						throw new Exception("There seem to be more than one SimulationManager active???");
					}
					foundSimulation = true;
					newList.Add(new CustomTicker(startSimulationTick));
					newList.Add(ticker);
					newList.Add(new CustomTicker(stopSimulationTick));
				}
				else
				{
					newList.Add(ticker);
				}
			}
			newList.Add(new CustomTicker(stopServerTick));
			if(!foundSimulation)
			{
				throw new Exception("Could not find SimulationManager ticker service...");
			}
			field.SetValue(server, newList);
		}
		
		private class CustomTicker : IServerTicker
		{
			public int TickPriority => -100;
			
			private readonly Action callback;
			
			public CustomTicker(Action callback)
			{
				this.callback = callback;
			}
			
			public void Tick(double deltaTime)
			{
				callback();
			}
		}
		
		//Component logic:
		
		private static long lastValue; //First server sleep will be wrong, but that is okay.
		
		public static bool simulationRunning;
		
		public static int serverTickRoundCounter; //Used for components to detect server tick cycle changes reliably.
		
		public static long timeOfSimulationTickStart;
		public static int skippedServerTicks;
		public static long timeSinceServerTickStop;
		public static long timeSinceServerTickStart;
		public static long timeSinceSimulationTickStart;
		public static long timeSinceSimulationTickStop;
		
		private static void startServerTick()
		{
			var stamp = Stopwatch.GetTimestamp();
			var delta = stamp - lastValue;
			lastValue = stamp;
			
			serverTickRoundCounter++; //Start a new round.
			timeSinceServerTickStop += delta;
		}
		
		private static void startSimulationTick()
		{
			var stamp = Stopwatch.GetTimestamp();
			var delta = stamp - lastValue;
			lastValue = stamp;
			
			timeOfSimulationTickStart = stamp;
			timeSinceServerTickStart += delta;
		}
		
		//Components will access the data during the simulation tick.
		
		private static void stopSimulationTick()
		{
			var stamp = Stopwatch.GetTimestamp();
			var delta = stamp - lastValue;
			lastValue = stamp;
			
			//Cleanup all fields, if the simulation was running:
			if(simulationRunning)
			{
				//Simulation was running, meaning all values got consumed by a stopwatch component.
				// Reset to provide delta values next time round.
				timeSinceServerTickStop = 0; //Idle
				timeSinceServerTickStart = 0; //Server
				timeSinceSimulationTickStart = 0; //Simulation
				timeSinceSimulationTickStop = 0; //Server
				skippedServerTicks = 0;
				
				simulationRunning = false;
			}
			else
			{
				skippedServerTicks += 1;
			}
			
			timeSinceSimulationTickStart += delta;
		}
		
		private static void stopServerTick()
		{
			var stamp = Stopwatch.GetTimestamp();
			var delta = stamp - lastValue;
			lastValue = stamp;
			
			timeSinceSimulationTickStop += delta;
		}
		
		public static string toMillis(long value)
		{
			return round(value * FACTOR_TIME_TO_MILLIS) + "ms";
		}
		
		public static string round(double value)
		{
			value *= 100;
			value = Math.Round(value);
			value /= 100;
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}
