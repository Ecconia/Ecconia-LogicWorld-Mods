using EccsLogicWorldAPI.Server;
using LogicAPI.Server.Components;
using LogicWorld.Server.HostServices;

namespace EcconiaCPUServerComponents.Server
{
	public class TPSPrinter : LogicComponent
	{
		private static readonly ISimulationManager simulation;
		
		static TPSPrinter()
		{
			simulation = ServiceGetter.getService<ISimulationManager>();
		}
		
		private int lastTPS;
		
		protected override void DoLogicUpdate()
		{
			QueueLogicUpdate(); //We gonna run every tick.
			
			var currentTPS = (int) simulation.TicksPerSecond;
			if(lastTPS != currentTPS)
			{
				lastTPS = currentTPS;
				
				//Update outputs:
				if(currentTPS > 0xFFFF)
				{
					currentTPS = 0xFFFF;
				}
				var mask = 1;
				for(var i = 0; i < 16; i++)
				{
					Outputs[i].On = (currentTPS & mask) != 0;
					mask <<= 1;
				}
			}
		}
	}
}
