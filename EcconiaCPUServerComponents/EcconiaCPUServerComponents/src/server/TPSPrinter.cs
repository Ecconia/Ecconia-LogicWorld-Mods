using System;
using LogicAPI.Server.Components;
using LogicWorld.Server;
using LogicWorld.Server.HostServices;

namespace EcconiaCPUServerComponents.Server
{
	public class TPSPrinter : LogicComponent
	{
		private static readonly PeriodicTicker simulation;
		
		static TPSPrinter()
		{
			simulation = (PeriodicTicker) Program.Get<ISimulationService>(); //Just do the dirty casting, we need 'IsPaused'.
			if(simulation == null)
			{
				throw new Exception("Could not get simulation service. /tps will break.");
			}
		}
		
		private int lastTPS;
		
		protected override void DoLogicUpdate()
		{
			QueueLogicUpdate(); //We gonna run every tick.
			
			int currentTPS = (int) simulation.TicksPerSecond;
			if(lastTPS != currentTPS)
			{
				lastTPS = currentTPS;
				
				//Update outputs:
				if(currentTPS > 0xFFFF)
				{
					currentTPS = 0xFFFF;
				}
				int mask = 1;
				for(int i = 0; i < 16; i++)
				{
					Outputs[i].On = (currentTPS & mask) != 0;
					mask <<= 1;
				}
			}
		}
	}
}
