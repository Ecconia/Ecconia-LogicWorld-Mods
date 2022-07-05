using System.Linq;
using System.Reflection;
using HarmonyLib;
using LogicAPI.Data;
using LogicAPI.Services;
using LogicWorld.Server;
using LogicWorld.Server.Circuitry;
using LogicWorld.Server.Saving;

namespace ServerLoadAccelerator.server
{
	public class InitWireClustersLater
	{
		private const string harmonyID = "ServerInitialLoadAccelerator";
		private static Harmony harmony;
		private static ICircuitryManager circuitManager;

		public static void init()
		{
			circuitManager = Program.Get<ICircuitryManager>();
			if(circuitManager == null)
			{
				ModClass.logger.Error("Could not get access to 'ICircuitManager'. Cannot speedup world loading.");
				return;
			}

			harmony = new Harmony(harmonyID);

			//General scope of execution:
			MethodInfo loadMethod = ReflectionHelper.getMethod(typeof(SaveManager), "ReloadActiveSave");
			MethodInfo patchStartLoad = ReflectionHelper.getMethod(typeof(InitWireClustersLater), nameof(startLoad));
			MethodInfo patchStopLoad = ReflectionHelper.getMethod(typeof(InitWireClustersLater), nameof(stopLoad));

			//Intercept this method:
			MethodInfo wireCircuitMethod = ReflectionHelper.getMethod(typeof(CircuitryManager), nameof(CircuitryManager.UpdateCircuitModelForNewWire));
			MethodInfo patchWireCircuit = ReflectionHelper.getMethod(typeof(InitWireClustersLater), nameof(patchWireCircuitPrefix));

			//Start and stop hooks:
			MethodInfo firstWireHook = ReflectionHelper.getMethod(typeof(Program), "LogicWorld.Server.Managers.ServerWorldDataMutator", "AddNewWire");
			MethodInfo patchFirstWireHook = ReflectionHelper.getMethod(typeof(InitWireClustersLater), nameof(firstWire));
			MethodInfo postWiresHookFirst = ReflectionHelper.getMethod(typeof(CircuitStates), "SetStateNoQueueing");
			MethodInfo patchPostWiresHookFirst = ReflectionHelper.getMethod(typeof(InitWireClustersLater), nameof(postWireFirst));
			MethodInfo postWiresHookSecond = ReflectionHelper.getMethod(typeof(LogicManager), "DoLogicUpdate");
			MethodInfo patchPostWiresHookSecond = ReflectionHelper.getMethod(typeof(InitWireClustersLater), nameof(postWireSecond));

			//Do the patching only, after everything required has been gathered.
			harmony.Patch(loadMethod, new HarmonyMethod(patchStartLoad), new HarmonyMethod(patchStopLoad));
			harmony.Patch(wireCircuitMethod, new HarmonyMethod(patchWireCircuit));
			harmony.Patch(firstWireHook, new HarmonyMethod(patchFirstWireHook));
			harmony.Patch(postWiresHookFirst, new HarmonyMethod(patchPostWiresHookFirst));
			harmony.Patch(postWiresHookSecond, new HarmonyMethod(patchPostWiresHookSecond));
		}

		private static bool active;
		private static bool workingOnWires;
		private static bool doneWithWires;
		private static IWorldData currentWorld;

		private static void startLoad(IWorldDataMutator ___WorldDataMutator)
		{
			active = true;
			currentWorld = ___WorldDataMutator.Data;
		}

		private static bool patchWireCircuitPrefix()
		{
			return !(active && workingOnWires);
		}

		private static void firstWire()
		{
			if(active && !workingOnWires)
			{
				workingOnWires = true;
			}
		}

		private static void postWireFirst()
		{
			if(active && !doneWithWires)
			{
				doneWithWires = true;
				afterWires();
			}
		}

		private static void postWireSecond()
		{
			if(active && !doneWithWires)
			{
				doneWithWires = true;
				afterWires();
			}
		}

		private static void afterWires()
		{
			workingOnWires = false;
			{
				//Wire processing:
				foreach(var wire in currentWorld.AllWires.Values)
				{
					wire.StateID = -1;
				}
				foreach(var entry in currentWorld.AllWires)
				{
					Wire wire = entry.Value;
					PegAddress point1 = wire.Point1;
					PegAddress point2 = wire.Point2;
					if(wire.StateID == -1 || (point1.IsInput && !point2.IsInput || !point1.IsInput && point2.IsInput) && currentWorld.LookupPegWires(point1.IsInput ? point1 : point2).Any())
					{
						//Either unprocessed, or a critical wire - that got overwritten by another cluster creation...
						circuitManager.UpdateCircuitModelForNewWire(entry.Key);
						if(wire.StateID == -1)
						{
							//Well clients will certainly not like a negative circuit state index
							ModClass.logger.Warn("Failed to process wire: " + entry.Key + " disable this mod and restart, please report this issue.");
						}
					}
				}
			}
		}

		private static void stopLoad()
		{
			active = false;
			workingOnWires = false;
			doneWithWires = false;
			currentWorld = null;

			harmony.UnpatchAll(harmonyID);
			harmony = null;
		}
	}
}
