using System.Collections.Generic;
using LogicAPI.Server.Components;

namespace RadioConnection.Server
{
	/**
	 * Implement this interface in your component, if you wish to interact with the Channel system of this mod.
	 * After just do what the RadioComponent does and register/unregister your component to a channel via the ChannelManager.
	 *
	 * It is your responsibility to always unregister the channel again and to never let the peg count change while being linked!
	 */
	public interface RadioCapable
	{
		//Index of the first input peg in your component that represents a data bit:
		int getPegOffset();
		//All input pegs of your component:
		IReadOnlyList<IInputPeg> getPegs();
		//Tuple of an offset of bits in the channel and the amount of bits to link to your pegs:
		// Ensure that the amount of bits is never higher than your peg count minus the peg offset!
		(int, int) getBitRange();
		//If this component should be linked using phasic link (this is only the case if the channel frequently unlinks and links again):
		bool isUsingPhasicLinks();
	}
}
