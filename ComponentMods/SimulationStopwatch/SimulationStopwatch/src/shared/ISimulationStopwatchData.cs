namespace SimulationStopwatch.Shared
{
	public interface ISimulationStopwatchData
	{
		string sessionName { get; set; }
		bool printDebugEveryServerTick { get; set; }
	}
	
	public static class Initialize
	{
		public static void initialize(this ISimulationStopwatchData data)
		{
			data.sessionName = "Random Test";
			data.printDebugEveryServerTick = false;
		}
	}
}
