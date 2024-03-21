using System;
using System.Collections.Generic;
using System.Linq;
using LogicAPI.Data;
using LogicWorld.SharedCode.Saving;
using LogicWorld.SharedCode.Saving.Data;

namespace EccsLogicWorldAPI.Server.Generators
{
	public static class UnusedComponentAddressGrabber
	{
		private static readonly Queue<ComponentAddress> unusedComponentAddresses = new Queue<ComponentAddress>();
		
		private static int grabAddressesAmount;
		private static bool initialized;
		
		public static void grabOneMore()
		{
			grabAddressesAmount += 1;
			initialize();
		}
		
		public static ComponentAddress getUnusedComponentAddress()
		{
			return unusedComponentAddresses.Count > 0 ? unusedComponentAddresses.Dequeue() : ComponentAddressGrabber.getNewComponentAddress();
		}
		
		// ======================
		
		private static void initialize()
		{
			if(initialized)
			{
				return;
			}
			initialized = true;
			
			SaveReader.PostParseSaveTransformers += unusedIDCollector;
		}
		
		private static void unusedIDCollector(SaveFileData saveFileData)
		{
			if(saveFileData.BaseInfo.SaveType != SaveType.World)
			{
				return; // We need to listen to the main world only.
			}
			
			// Collect all IDs in a sorted array, expensive but unavoidable to find gaps:
			var entries = saveFileData.ObjectData.SavedComponentDatas;
			uint[] arr = entries.Select(e => e.cAddress.ID).ToArray();
			Array.Sort(arr);
			
			// Assume, that there is always one address to find, as else this would not be called.
			
			uint nextExpectedAddress = 1; // 0 does not exist. Start at 1.
			foreach(var address in arr)
			{
				if(address == nextExpectedAddress)
				{
					// No gap since last address, expect the next one without gap.
					nextExpectedAddress += 1;
					continue;
				}
				// Found a gap!
				var amountOfUnusedAddresses = address - nextExpectedAddress;
				var addressesToGrab = (int) Math.Min(grabAddressesAmount, amountOfUnusedAddresses);
				for(int i = 0; i < addressesToGrab; i++)
				{
					unusedComponentAddresses.Enqueue(new ComponentAddress(nextExpectedAddress));
					nextExpectedAddress += 1;
				}
				// Subtract what we gathered from what we have to gather and stop once we gathered enough.
				grabAddressesAmount -= addressesToGrab;
				if(grabAddressesAmount <= 0)
				{
					break;
				}
				// We need to increment one more time, to go from address to address+1:
				nextExpectedAddress += 1;
			}
			
			// Done, remove the hook.
			SaveReader.PostParseSaveTransformers -= unusedIDCollector;
		}
	}
}
