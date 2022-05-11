using System.Collections.Generic;
using System.Linq;
using LogicAPI.Data;
using LogicWorld.Audio;
using LogicWorld.Outlines;
using UnityEngine;

namespace CustomWirePlacer.Client.CWP
{
	public class CWPGroupAxis
	{
		public PegAddress firstPeg;
		public PegAddress secondPeg;

		public IEnumerable<PegAddress> inBetween;

		public IEnumerable<PegAddress> forwards;
		public IEnumerable<PegAddress> backwards;

		private bool binarySkipping;
		private int skipNumber = 1;
		//TODO: Add an offset to skipping.

		public void clear()
		{
			hide();
			firstPeg = secondPeg = null;
			inBetween = forwards = backwards = null;
			skipNumber = 1;
			binarySkipping = false;
		}
		
		public void hide()
		{
			Outliner.RemoveHardOutline(firstPeg);
			Outliner.RemoveHardOutline(secondPeg);
			if(inBetween != null)
			{
				Outliner.RemoveHardOutline(inBetween);
			}
			if(forwards != null)
			{
				Outliner.RemoveHardOutline(forwards);
			}
			if(backwards != null)
			{
				Outliner.RemoveHardOutline(backwards);
			}
		}

		public void show()
		{
			int skipIndex = getSkipStart(); //Start with skip-number, because the first peg is always chosen.
			if(backwards != null)
			{
				foreach(PegAddress peg in backwards)
				{
					Outliner.HardOutline(peg, isNotSkipped(ref skipIndex) ? CWPOutlineData.firstDiscoveredPegs : CWPOutlineData.skippedPeg);
				}
			}
			if(firstPeg != null)
			{
				Outliner.HardOutline(firstPeg, isNotSkipped(ref skipIndex) ? CWPOutlineData.firstPeg : CWPOutlineData.firstSkippedPeg);
			}
			if(inBetween != null)
			{
				foreach(PegAddress peg in inBetween)
				{
					Outliner.HardOutline(peg, isNotSkipped(ref skipIndex) ? CWPOutlineData.middlePegs : CWPOutlineData.skippedPeg);
				}
			}
			if(secondPeg != null)
			{
				Outliner.HardOutline(secondPeg, isNotSkipped(ref skipIndex) ? CWPOutlineData.secondPeg : CWPOutlineData.secondSkippedPeg);
			}
			if(forwards != null)
			{
				foreach(PegAddress peg in forwards)
				{
					Outliner.HardOutline(peg, isNotSkipped(ref skipIndex) ? CWPOutlineData.secondDiscoveredPegs : CWPOutlineData.skippedPeg);
				}
			}
		}
		
		public void setSecondPeg(PegAddress secondPeg) //Nullable
		{
			hide(); //Hide all visible outlines, since some might be removed.

			this.secondPeg = secondPeg;
			if(secondPeg != null)
			{
				inBetween = CWPHelper.collectPegsInBetween(firstPeg, secondPeg);
				backwards = forwards = null; //These get reset, now that their axis might have changed.
			}

			show(); //Redraw all visible outlines, respecting the skip number.
		}

		public IEnumerable<PegAddress> getPegs()
		{
			int skipIndex = getSkipStart(); //Start with skipNumber, to always select the first peg.
			if(backwards != null)
			{
				foreach(PegAddress peg in backwards)
				{
					if(isNotSkipped(ref skipIndex))
					{
						yield return peg;
					}
				}
			}
			if(isNotSkipped(ref skipIndex))
			{
				yield return firstPeg;
			}
			if(inBetween != null)
			{
				foreach(PegAddress peg in inBetween)
				{
					if(isNotSkipped(ref skipIndex))
					{
						yield return peg;
					}
				}
			}
			if(secondPeg != null)
			{
				if(isNotSkipped(ref skipIndex))
				{
					yield return secondPeg;
				}
			}
			if(forwards != null)
			{
				foreach(PegAddress peg in forwards)
				{
					if(isNotSkipped(ref skipIndex))
					{
						yield return peg;
					}
				}
			}
		}
		
		//TODO: Discord this, and do properly respect of skipping when in-line MWP.
		public IEnumerable<PegAddress> getAllPegs()
		{
			if(backwards != null)
			{
				foreach(PegAddress peg in backwards)
				{
					yield return peg;
				}
			}
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
			if(forwards != null)
			{
				foreach(PegAddress peg in forwards)
				{
					yield return peg;
				}
			}
		}

		private int getSkipStart()
		{
			return binarySkipping ? 0 : skipNumber;
		}

		private bool isNotSkipped(ref int skipIndex)
		{
			if(binarySkipping)
			{
				//TODO: Support non binary numbers.
				if(skipNumber == 1)
				{
					return true;
				}
				return (skipIndex++ & (1 << (skipNumber - 2))) != 0;
			}
			else
			{
				if(skipIndex++ == skipNumber)
				{
					skipIndex = 1;
					return true;
				}
				return false;
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
		
		public void switchSkipMode()
		{
			hide();
			binarySkipping = !binarySkipping;
			show();
		}
		
		public void expandFurther()
		{
			if(secondPeg == null)
			{
				return;
			}
			hide();
			expandFurtherInternal();
			show();
		}

		private void expandFurtherInternal()
		{
			PegAddress start = firstPeg;
			PegAddress end = secondPeg;
			if(inBetween != null)
			{
				start = inBetween.Last();
			}
			Vector3 startPos = CWPHelper.getWireConnectionPoint(start);
			Vector3 endPos = CWPHelper.getWireConnectionPoint(end);
			Vector3 ray = endPos - startPos;
			forwards = CWPHelper.collectPegsInDirection(endPos, ray);
		}

		public void expandBackwards()
		{
			if(secondPeg == null)
			{
				return;
			}
			hide();
			expandBackwardsInternal();
			show();
		}

		private void expandBackwardsInternal()
		{
			PegAddress start = secondPeg;
			PegAddress end = firstPeg;
			if(inBetween != null)
			{
				start = inBetween.First();
			}
			Vector3 startPos = CWPHelper.getWireConnectionPoint(start);
			Vector3 endPos = CWPHelper.getWireConnectionPoint(end);
			Vector3 ray = endPos - startPos;
			backwards = CWPHelper.collectPegsInDirection(endPos, ray);
			if(backwards != null)
			{
				backwards = backwards.Reverse(); //We casted from the wrong direction, so these pegs need to be reversed.
			}
		}
		
		public void applyAxis(CWPGroupAxis otherAxis, PegAddress firstPeg)
		{
			//No second peg, custom code for that:
			if(otherAxis.secondPeg == null)
			{
				clear();
				this.firstPeg = firstPeg;
				this.binarySkipping = otherAxis.binarySkipping;
				show();
				return; //Done.
			}

			//Second peg:
			PegAddress secondPeg = CWPHelper.getPegRelativeToOtherPeg(firstPeg, otherAxis.firstPeg, otherAxis.secondPeg);
			if(secondPeg == null)
			{
				//Cannot continue here... Don't clear data.
				SoundPlayer.PlayFail();
				return;
			}

			clear();
			this.firstPeg = firstPeg;
			this.secondPeg = secondPeg;
			this.skipNumber = otherAxis.skipNumber;
			this.binarySkipping = otherAxis.binarySkipping;
			//In between:
			this.inBetween = CWPHelper.collectPegsInBetween(firstPeg, secondPeg);
			//Backwards:
			if(otherAxis.backwards != null)
			{
				expandBackwardsInternal();
			}
			//Forwards:
			if(otherAxis.forwards != null)
			{
				expandFurtherInternal();
			}

			show();
		}
	}
}
