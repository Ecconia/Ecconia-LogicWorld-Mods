using System;
using System.Collections.Generic;
using System.Linq;
using EccsLogicWorldAPI.Server;
using LogicWorld.Server.Circuitry;

namespace RadioConnection.Server
{
	public class Channel
	{
		private readonly uint index;
		private readonly List<RadioCapable> clients = new List<RadioCapable>();
		private readonly List<InputPeg> bits = new List<InputPeg>();
		
		private DateTime? timeWhenEmpty;
		
		public Channel(uint index)
		{
			this.index = index;
		}
		
		public uint getIndex()
		{
			return index;
		}
		
		public DateTime? getEmptyTime()
		{
			return timeWhenEmpty;
		}
		
		public void destroy()
		{
			if(clients.Count != 0)
			{
				throw new Exception("Expected all clients to be removed, before destroying a channel. Something went very wrong.");
			}
			foreach(var peg in bits)
			{
				if(peg != null)
				{
					VirtualInputPegPool.returnPeg(peg);
				}
			}
			bits.Clear();
		}
		
		public void register(RadioCapable component)
		{
			timeWhenEmpty = null; //Reset deletion timer (if it was set).
			
			//About to link the second component, so also link the previously skipped first one:
			if(clients.Count == 1)
			{
				// LConsole.WriteLine("_Link-First");
				link(clients.First());
			}
			clients.Add(component);
			//Do not link the component, if it is the only component in this channel (prevents overhead and expanding the bits/pegs list):
			if(clients.Count > 1)
			{
				// LConsole.WriteLine("_Link-New");
				link(component);
			}
			// LConsole.WriteLine("_Link/Done");
		}
		
		public void unregister(RadioCapable component)
		{
			//If there is only one component, that one was/is not linked, no need to unlink it.
			if(clients.Count > 1)
			{
				// LConsole.WriteLine("_Unlink-New");
				unlink(component);
			}
			clients.Remove(component);
			//By convention if a component is the only one in the channel, it has to be unlinked. Does not make that much sense here, but makes code less complex. Overhead is small-ish.
			if(clients.Count == 1)
			{
				// LConsole.WriteLine("_Unlink-Last");
				unlink(clients.First());
			}
			// LConsole.WriteLine("_Unlink/Done");
			//All components removed, schedule for removal.
			if(clients.Count == 0)
			{
				timeWhenEmpty = DateTime.UtcNow;
			}
		}
		
		private void link(RadioCapable component)
		{
			var firstPeg = component.getPegOffset();
			var pegs = component.getPegs();
			var (firstIndex, dataLength) = component.getBitRange();
			var isPhasic = component.isUsingPhasicLinks();
			
			var limit = firstIndex + dataLength;
			while(bits.Count <= limit)
			{
				bits.Add(null);
			}
			for(int bitIndex = firstIndex, dataPeg = firstPeg; bitIndex < limit; bitIndex++, dataPeg++)
			{
				var bitPeg = bits[bitIndex];
				if(bitPeg == null)
				{
					bitPeg = VirtualInputPegPool.borrowPeg();
					bits[bitIndex] = bitPeg;
				}
				if(isPhasic)
				{
					bitPeg.AddPhasicLinkWithUnsafe(pegs[dataPeg]);
				}
				else
				{
					bitPeg.AddSecretLinkWith(pegs[dataPeg]);
				}
			}
		}
		
		private void unlink(RadioCapable component)
		{
			var firstPeg = component.getPegOffset();
			var pegs = component.getPegs();
			var (firstIndex, dataLength) = component.getBitRange();
			var isPhasic = component.isUsingPhasicLinks();
			
			if(bits.Count < firstIndex)
			{
				return; //Ehm nothing to do here.
			}
			
			var limit = firstIndex + dataLength;
			if(bits.Count < limit)
			{
				limit = bits.Count;
			}
			for(int bitIndex = firstIndex, dataPeg = firstPeg; bitIndex < limit; bitIndex++, dataPeg++)
			{
				var bitPeg = bits[bitIndex];
				if(bitPeg == null)
				{
					continue;
				}
				if(isPhasic)
				{
					bitPeg.RemovePhasicLinkWithUnsafe(pegs[dataPeg]);
				}
				else
				{
					bitPeg.RemoveSecretLinkWith(pegs[dataPeg]);
				}
				if(bitPeg.SecretLinks != null && bitPeg.SecretLinks.Count == 0 && bitPeg.PhasicLinks != null && bitPeg.PhasicLinks.Count == 0)
				{
					bits[bitIndex] = null;
					VirtualInputPegPool.returnPeg(bitPeg);
				}
			}
		}
	}
}
