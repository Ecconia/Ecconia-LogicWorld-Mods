using System;
using System.Collections.Generic;
using EccsLogicWorldAPI.Server.Generators;
using EccsLogicWorldAPI.Shared;
using LogicAPI.Data;
using LogicWorld.Server.Circuitry;

namespace EccsLogicWorldAPI.Server
{
	public static class VirtualInputPegPool
	{
		private static readonly Stack<InputPeg> pegs;
		
		private static ComponentAddress rootAddress;
		private static bool initialized;
		
		private static int count;
		
		static VirtualInputPegPool()
		{
			//Initialize:
			pegs = new Stack<InputPeg>();
			
			//Setup Harmony trap:
			try
			{
				HarmonyAtRuntime.init();
			}
			catch(Exception e)
			{
				throw new Exception("[EccLwApi/VirtualInputPegPool] This API feature requires Harmony to be installed. If it is installed, something is broken. See stacktrace for further debugging.", e);
			}
		}
		
		public static void ensureInitialized()
		{
			if(initialized)
			{
				return;
			}
			initialized = true;
			UnusedComponentAddressGrabber.grabOneMore();
		}
		
		public static InputPeg borrowPeg()
		{
			if(pegs.TryPop(out var peg))
			{
				return peg;
			}
			if(rootAddress == ComponentAddress.Empty)
			{
				// Initialize root address:
				rootAddress = UnusedComponentAddressGrabber.getUnusedComponentAddress();
			}
			return InputPegFactory.generateNewInputPeg(new InputAddress(rootAddress, count++));
		}
		
		/**
		 * Return a peg previously borrowed back to the pool.
		 * MAKE SURE TO REMOVE EVERY CONNECTION TO THIS PEG BEFORE RETURNING!
		 */
		public static void returnPeg(InputPeg peg)
		{
			if(peg == null)
			{
				throw new Exception("Cannot return 'null' input peg.");
			}
			if(peg.Address.ComponentAddress != rootAddress)
			{
				throw new Exception("Only return pegs that are borrowed from this pool. Got peg with component address " + peg.Address.ComponentAddress.ID + " while should be " + rootAddress.ID);
			}
			pegs.Push(peg);
		}
	}
}
