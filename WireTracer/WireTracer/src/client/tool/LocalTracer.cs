using System.Collections.Generic;
using LogicAPI.Data;
using LogicWorld.Outlines;

namespace WireTracer.Client.Tool
{
	public class LocalTracer : GenericTracer
	{
		private static IEnumerable<(WireAddress, bool)> wires;
		private static IEnumerable<PegAddress> pegs;
		private static IEnumerable<ComponentAddress> comps;

		public LocalTracer(PegAddress origin)
		{
			(pegs, wires, comps) = BruteForceCollector.collect(origin);

			//First components, so that the pegs can be overwritten.
			foreach(var componentAddress in comps)
			{
				Outliner.Outline(componentAddress, WireTracerColors.primaryConnected);
			}
			foreach(var pAddress in pegs)
			{
				Outliner.Outline(pAddress, pAddress.IsInput ? WireTracerColors.primaryNormal : WireTracerColors.primaryOutput);
			}
			foreach(var (wireAddress, isOutput) in wires)
			{
				Outliner.Outline(wireAddress, isOutput ? WireTracerColors.primaryOutput : WireTracerColors.primaryNormal);
			}
		}

		public void stop()
		{
			foreach(var (wireAddress, _) in wires)
			{
				Outliner.RemoveOutline(wireAddress);
			}
			foreach(var pegAddress in pegs)
			{
				Outliner.RemoveOutline(pegAddress);
			}
			foreach(var componentAddress in comps)
			{
				Outliner.RemoveOutline(componentAddress);
			}
		}
	}
}
