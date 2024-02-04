using System;
using System.Collections.Generic;
using System.Reflection;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI.Data;
using LogicAPI.Server.Components;
using LogicWorld.Server.Circuitry;

namespace EccsLogicWorldAPI.Server.Generators
{
	public static class InputPegFactory
	{
		private static readonly ICircuitryManager iCircuitryManager;
		private static readonly IClusterFactory iClusterFactory;
		private static readonly CircuitStates circuitStates;
		private static readonly Func<ICircuitryManager, Dictionary<InputAddress, InputPeg>> getLogicInputs;
		private static readonly Func<InputAddress, LogicComponent, bool, CircuitStates, ICircuitryManager, InputPeg> newInputPeg;
		
		static InputPegFactory()
		{
			iCircuitryManager = ServiceGetter.getService<ICircuitryManager>();
			iClusterFactory = ServiceGetter.getService<IClusterFactory>();
			circuitStates = ServiceGetter.getService<CircuitStates>();
			getLogicInputs = Delegator.createFieldGetter<ICircuitryManager, Dictionary<InputAddress, InputPeg>>(Fields.getPrivate(typeof(CircuitryManager), "LogicInputs"));
			var constructor = typeof(InputPeg).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[]
			{
				typeof(InputAddress),
				typeof(LogicComponent),
				typeof(bool),
				typeof(CircuitStates),
				typeof(ICircuitryManager),
			}, null);
			if(constructor == null)
			{
				throw new Exception("Could not find constructor of object InputPeg.");
			}
			newInputPeg = Delegator.createObjectInitializer<InputAddress, LogicComponent, bool, CircuitStates, ICircuitryManager, InputPeg>(constructor);
		}
		
		/**
		 * Will generate a new "virtual" InputPeg.
		 * This peg won't have a physical representation.
		 * But is a full part of the circuit model.
		 * Thus it can be used to route signals invisibly.
		 *
		 * WARNING: YOU ARE RESPONSIBLE FOR THIS PEG! POSSIBLE MEMORY LEAK!
		 *  This peg must be destroyed again (once unused), or you will have a memory leak.
		 *
		 * InputAddress - The address which this pegs belongs to.
		 *  If it is bound to a component, one can simply use the component address for this peg, but the index must not collide with other pegs on the component.
		 * In case you plan to have the peg not bound to a component,
		 *  I advise you to use the 'VirtualInputPegPool' provided by this mod. It reserves one address for all virtual pegs and generates the InputAddress too.
		 *
		 * Also, this code will not work without the patch that 'VirtualInputPegPool' does. If you need this, feel free to talk to Ecconia.
		 */
		public static InputPeg generateNewInputPeg(InputAddress address)
		{
			//Create peg:
			var input = newInputPeg(
				address,
				null,
				false,
				circuitStates,
				iCircuitryManager
			);
			//Register peg:
			getLogicInputs(iCircuitryManager).Add(address, input);
			//Create cluster for peg:
			iClusterFactory.CreateStarter(input);
			return input;
		}
	}
}
