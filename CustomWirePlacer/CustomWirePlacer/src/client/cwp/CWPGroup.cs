using System.Collections.Generic;
using System.Linq;
using LogicAPI.Data;
using LogicWorld.Outlines;

namespace CustomWirePlacer.Client.CWP
{
	public class CWPGroup
	{
		private PegAddress firstPeg;
		private PegAddress secondPeg;

		private IEnumerable<PegAddress> inBetween;

		public void clear()
		{
			Outliner.RemoveHardOutline(firstPeg);
			Outliner.RemoveHardOutline(secondPeg);
			firstPeg = secondPeg = null;
			if(inBetween != null)
			{
				Outliner.RemoveHardOutline(inBetween);
				inBetween = null;
			}
		}

		public bool isSet()
		{
			return firstPeg != null;
		}

		public void setFirstPeg(PegAddress firstPeg)
		{
			//If dirty call, handle anyway:
			if(this.firstPeg != null)
			{
				//Well... lets just clear it - should not have happened.
				clear();
			}

			//Expecting this to only happen once after clearing.
			this.firstPeg = firstPeg;

			//Outline the first peg:
			Outliner.HardOutline(firstPeg, CWPOutlineData.firstPeg);
		}

		public PegAddress getFirstPeg()
		{
			return firstPeg;
		}

		public void setSecondPeg(PegAddress secondPeg) //Nullable
		{
			if(inBetween != null)
			{
				Outliner.RemoveHardOutline(inBetween);
				inBetween = null;
			}

			if(this.secondPeg != null)
			{
				Outliner.RemoveHardOutline(this.secondPeg);
			}
			this.secondPeg = secondPeg;

			Outliner.HardOutline(secondPeg, CWPOutlineData.secondPeg);

			if(secondPeg != null)
			{
				inBetween = CWPHelper.collectPegsInBetween(firstPeg, secondPeg);
				if(inBetween != null)
				{
					Outliner.HardOutline(inBetween.ToList().AsReadOnly(), CWPOutlineData.middlePegs);
				}
			}
		}

		public PegAddress getSecondPeg()
		{
			return secondPeg;
		}

		public bool hasExtraPegs()
		{
			//TODO: Check other collections, like the discovery one. But in the end the cached selection has to be checked.
			return inBetween != null;
		}

		//Should only be called when the content has changed, since quite expensive.
		public IEnumerable<PegAddress> getPegs()
		{
			yield return firstPeg;
			if(inBetween != null)
			{
				foreach(PegAddress peg in inBetween)
				{
					yield return peg;
				}
			}
			if(secondPeg != null)
			{
				yield return secondPeg;
			}
		}
	}
}
