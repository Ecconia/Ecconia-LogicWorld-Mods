using System.Collections.Generic;
using LogicAPI.Data;
using LogicWorld.Outlines;

namespace CustomWirePlacer.Client.CWP
{
	public class CWPGroup
	{
		private PegAddress firstPeg;
		private PegAddress secondPeg;

		private IEnumerable<PegAddress> inBetween;

		private int skipNumber = 1;

		public void clear()
		{
			hide();
			firstPeg = secondPeg = null;
			inBetween = null;
			skipNumber = 1;
		}

		public void hide()
		{
			Outliner.RemoveHardOutline(firstPeg);
			Outliner.RemoveHardOutline(secondPeg);
			if(inBetween != null)
			{
				Outliner.RemoveHardOutline(inBetween);
			}
		}

		public void show()
		{
			int skipIndex = skipNumber; //Start with skip-number, because the first peg is always chosen.
			if(firstPeg != null)
			{
				bool isNotSkipped = skipIndex++ == skipNumber;
				if(isNotSkipped)
				{
					skipIndex = 1;
				}
				Outliner.HardOutline(firstPeg, isNotSkipped ? CWPOutlineData.firstPeg : CWPOutlineData.firstSkippedPeg);
			}
			if(inBetween != null)
			{
				foreach(PegAddress peg in inBetween)
				{
					bool isNotSkipped = skipIndex++ == skipNumber;
					if(isNotSkipped)
					{
						skipIndex = 1;
					}
					Outliner.HardOutline(peg, isNotSkipped ? CWPOutlineData.middlePegs : CWPOutlineData.skippedPeg);
				}
			}
			if(secondPeg != null)
			{
				bool isNotSkipped = skipIndex++ == skipNumber;
				if(isNotSkipped)
				{
					skipIndex = 1;
				}
				Outliner.HardOutline(secondPeg, isNotSkipped ? CWPOutlineData.secondPeg : CWPOutlineData.secondSkippedPeg);
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
			hide(); //Hide all visible outlines, since some might be removed.
			
			this.secondPeg = secondPeg;
			if(secondPeg != null)
			{
				inBetween = CWPHelper.collectPegsInBetween(firstPeg, secondPeg);
			}
			
			show(); //Redraw all visible outlines, respecting the skip number.
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
			int skipIndex = skipNumber; //Start with skipNumber, to always select the first peg.
			if(skipIndex++ == skipNumber)
			{
				skipIndex = 1;
				yield return firstPeg;
			}

			if(inBetween != null)
			{
				foreach(PegAddress peg in inBetween)
				{
					if(skipIndex++ == skipNumber)
					{
						skipIndex = 1;
						yield return peg;
					}
				}
			}

			if(secondPeg != null)
			{
				if(skipIndex++ == skipNumber)
				{
					skipIndex = 1;
					yield return secondPeg;
				}
			}
		}

		//Ignores peg skipping, and gets used by 1-group MWP actions.
		public IEnumerable<PegAddress> getAllPegs()
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

		public bool updateSkipNumber(int value)
		{
			int oldValue = skipNumber;
			skipNumber += value;
			if(skipNumber < 1)
			{
				skipNumber = 1;
			}
			if(skipNumber != oldValue)
			{
				//Update all outlines:
				hide();
				show();
				return true;
			}
			return false;
		}
	}
}
