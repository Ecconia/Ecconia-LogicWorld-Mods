using System.Diagnostics;
using EccsLogicWorldAPI.Server;
using LogicWorld.Server.Circuitry;
using LogicWorld.Server.Networking;
using SimulationStopwatch.Shared;

namespace SimulationStopwatch.Server
{
	public class SimulationStopwatch : LogicComponent<ISimulationStopwatchData>
	{
		private static readonly ISenderShortcuts senderShortcuts;
		
		static SimulationStopwatch()
		{
			senderShortcuts = ServiceGetter.getService<ISenderShortcuts>();
		}
		
		//Session data:
		
		private string lastSessionName;
		private bool lastStartTriggerState;
		private bool lastStopTriggerState;
		
		private bool isActive;
		private int serverTickIndex;
		private int startServerTickIndex;
		
		//Statistic values:
		
		private int simulationTickCount;
		private int serverTickCount;
		private int ticksPerTickCounter;
		
		private long startTime;
		
		private long timeSimulationTick;
		private long timeServerTick;
		private long timeIdle;
		
		protected override void DoLogicUpdate()
		{
			bool startPeg = Inputs[0].On;
			bool stopPeg = Inputs[1].On;
			//Initialize:
			if(lastSessionName == null)
			{
				//Did not initialize component yet. Lets do so.
				lastSessionName = Data.sessionName;
				lastStartTriggerState = startPeg;
				lastStopTriggerState = stopPeg;
				return; //This is the first initial tick, that is performed on world loading. Do not do anything - besides setup.
			}
			//Detect rising edge control signals:
			bool isStart = startPeg && !lastStartTriggerState;
			bool isStop = stopPeg && !lastStopTriggerState;
			lastStartTriggerState = startPeg;
			lastStopTriggerState = stopPeg;
			
			if(!isActive && isStart && isStop)
			{
				return; //Not running, but both? No.
			}
			
			if(isStop && isActive)
			{
				stop();
				isActive = false;
			}
			
			if(isActive)
			{
				processTick();
			}
			
			if(isStart && !isActive)
			{
				start();
				isActive = true;
			}
			
			if(isActive)
			{
				//Continue running in the next tick to collect data there:
				QueueLogicUpdate();
			}
		}
		
		private void start()
		{
			ModClass.simulationRunning = true; //Make sure this is executed.
			
			startTime = Stopwatch.GetTimestamp();
			
			serverTickIndex = startServerTickIndex = ModClass.serverTickRoundCounter;
			simulationTickCount = 1;
			serverTickCount = 1;
			
			ticksPerTickCounter = 1;
			
			//Use a negative starting time for the simulation time, as in the next server tick
			timeSimulationTick = -(startTime - ModClass.timeOfSimulationTickStart);
			timeServerTick = 0;
			timeIdle = 0;
			
			senderShortcuts.DebugMessage(Data.sessionName + "> Start");
		}
		
		private void processTick()
		{
			simulationTickCount += 1;
			ticksPerTickCounter += 1;
			
			if(serverTickIndex == ModClass.serverTickRoundCounter)
			{
				return;
			}
			//This is the very first simulation tick, processed in this server tick.
			ModClass.simulationRunning = true;
			serverTickIndex = ModClass.serverTickRoundCounter;
			if(ModClass.skippedServerTicks != 0 && Data.printDebugEveryServerTick)
			{
				senderShortcuts.DebugMessage(Data.sessionName + "> SKIP " + ModClass.skippedServerTicks);
			}
			serverTickCount += ModClass.skippedServerTicks + 1;
			
			//Collect time statistics:
			timeSimulationTick += ModClass.timeSinceSimulationTickStart;
			timeServerTick += ModClass.timeSinceServerTickStart;
			timeServerTick += ModClass.timeSinceSimulationTickStop;
			timeIdle += ModClass.timeSinceServerTickStop;
			
			if(Data.printDebugEveryServerTick)
			{
				senderShortcuts.DebugMessage(Data.sessionName + "> ST>"
					+ " Ticks: " + ticksPerTickCounter
					+ " Simulation: " + ModClass.toMillis(ModClass.timeSinceSimulationTickStart)
					+ " Server: " + ModClass.toMillis(ModClass.timeSinceServerTickStart + ModClass.timeSinceSimulationTickStop)
					+ " Idle: " + ModClass.toMillis(ModClass.timeSinceServerTickStop)
					+ " T/SimT: " + (ticksPerTickCounter / (double) ModClass.timeSinceServerTickStart * ModClass.FACTOR_TIME_TO_MILLIS) + "ms"
				);
			}
			ticksPerTickCounter = 0;
		}
		
		private void stop()
		{
			long currentTime = Stopwatch.GetTimestamp();
			
			long totalRuntime = currentTime - startTime;
			if(serverTickIndex == startServerTickIndex)
			{
				//Still in the same tick:
				senderShortcuts.DebugMessage(Data.sessionName + "> Simulation ticks: " + simulationTickCount + " within one server tick");
				senderShortcuts.DebugMessage(Data.sessionName + "> Duration: " + ModClass.toMillis(totalRuntime));
				return;
			}
			//Advanced to the next server tick, provide advanced statistics:
			
			processTick();
			simulationTickCount -= 1; //Lets not count the tick required to update this component.
			timeSimulationTick += currentTime - ModClass.timeOfSimulationTickStart;
			
			senderShortcuts.DebugMessage(Data.sessionName + "> Simulation ticks: " + simulationTickCount + " Server ticks: " + serverTickCount);
			var onePercent = totalRuntime / 100.0;
			senderShortcuts.DebugMessage(Data.sessionName
				+ "> Total time: " + ModClass.toMillis(totalRuntime)
				+ " Simulation: " + ModClass.toMillis(timeSimulationTick) + " (" + ModClass.round(timeSimulationTick / onePercent) + "%)"
				+ " Server: " + ModClass.toMillis(timeServerTick) + " (" + ModClass.round(timeServerTick / onePercent) + "%)"
				+ " Idle: " + ModClass.toMillis(timeIdle) + " (" + ModClass.round(timeIdle / onePercent) + "%)"
			);
		}
		
		protected override void SetDataDefaultValues()
		{
			Data.initialize();
		}
	}
}
