using System.Collections.Generic;
using System.Reflection;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI.Server.Components;
using LogicWorld.Server.Circuitry;
using RadioConnection.Shared;

namespace RadioConnection.Server
{
	public class RadioComponent : LogicComponent<IRadioComponentData>, RadioCapable
	{
		private static readonly FieldInfo fieldInputPegShouldTriggerLogicUpdate;
		
		static RadioComponent()
		{
			fieldInputPegShouldTriggerLogicUpdate = Fields.getPrivate(typeof(InputPeg), "<" + "ShouldTriggerComponentLogicUpdates" + ">k__BackingField");
		}
		
		private uint? linkedChannel;
		
		//Variables to detect specific custom data changes:
		//For unlinking and peg changes:
		private uint lastAddressPegs;
		private uint lastDataPegs;
		//Just for unlinking:
		private bool lastLinkType;
		private uint lastAddressBase;
		private uint lastDataOffset;
		
		public int getPegOffset()
		{
			return (int) lastAddressPegs;
		}
		
		public IReadOnlyList<IInputPeg> getPegs()
		{
			return Inputs;
		}
		
		public (int, int) getBitRange()
		{
			return ((int) lastDataOffset, (int) lastDataPegs);
		}
		
		public bool isUsingPhasicLinks()
		{
			return lastAddressPegs != 0 && lastLinkType;
		}
		
		protected override void SetDataDefaultValues()
		{
			Data.initialize();
		}
		
		protected override void Initialize()
		{
			updateLastValues();
			//Update the bool inside the address pegs to cause logic updates.
			// LW does not consider custom data when originally setting this property.
			for(var i = 0; i < lastAddressPegs; i++)
			{
				fieldInputPegShouldTriggerLogicUpdate.SetValue(Inputs[i], true);
			}
		}
		
		private void updateLastValues()
		{
			lastAddressPegs = Data.addressPegs;
			lastDataPegs = Data.dataPegs;
			lastLinkType = Data.useLinkLayer;
			lastAddressBase = Data.addressBase;
			lastDataOffset = Data.dataOffset;
		}
		
		protected override void DoLogicUpdate()
		{
			//Called once resizing is done.
			//Called when the address pegs change.
			if(Data.addressPegs + Data.dataPegs != Inputs.Count)
			{
				//Which witchcraft is going on?
				return; //The initializing was not done properly or whatnot. This should never happen.
			}
			
			uint channelIndex = 0;
			if(Data.addressPegs != 0)
			{
				uint mask = 1;
				for(var i = 0; i < Data.addressPegs; i++)
				{
					if(Inputs[i].On)
					{
						channelIndex |= mask;
					}
					mask <<= 1;
				}
			}
			channelIndex += Data.addressBase;
			
			if(linkedChannel.HasValue && linkedChannel.Value != channelIndex)
			{
				ChannelManager.unlink(this, linkedChannel.Value);
				linkedChannel = null;
			}
			
			if(!linkedChannel.HasValue)
			{
				ChannelManager.linkChannel(this, channelIndex);
				linkedChannel = channelIndex;
			}
		}
		
		public override bool InputAtIndexShouldTriggerComponentLogicUpdates(int inputIndex)
		{
			//Only let address pegs update this component:
			return inputIndex < Data.addressPegs;
		}
		
		public override void OnComponentDestroyed()
		{
			if(linkedChannel.HasValue)
			{
				ChannelManager.unlink(this, linkedChannel.Value);
				linkedChannel = null;
			}
		}
		
		protected override void OnCustomDataUpdated()
		{
			if(lastDataPegs <= 0)
			{
				//Do not process anything here, before initialize was called.
				// Once it was called, this function is called automatically.
				return;
			}
			
			//Detect the two relevant types of custom data change:
			var pegLayoutChanged = Data.addressPegs != lastAddressPegs
				|| Data.dataPegs != lastDataPegs;
			var relevantCustomDataChange = pegLayoutChanged
				|| Data.dataOffset != lastDataOffset
				|| Data.addressBase != lastAddressBase
				|| Data.useLinkLayer != lastLinkType;
			
			if(relevantCustomDataChange && linkedChannel.HasValue)
			{
				ChannelManager.unlink(this, linkedChannel.Value);
				linkedChannel = null;
				QueueLogicUpdate(); //So that the channel gets linked again.
			}
			
			//Backup the old peg layout values:
			var oldAddressPegCount = lastAddressPegs;
			var oldDataPegCount = lastDataPegs;
			//Update the custom data change detection values:
			updateLastValues();
			
			if(pegLayoutChanged)
			{
				//Do the magic peg resizing without messing up wires:
				WireSetMutation.process(
					Address,
					Inputs,
					(int) oldAddressPegCount,
					(int) oldDataPegCount,
					(int) Data.addressPegs,
					(int) Data.dataPegs
				);
			}
		}
	}
}
