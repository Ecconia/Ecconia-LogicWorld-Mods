using System.Collections.Generic;
using System.Linq;
using LogicAPI.Data;
using LogicWorld.Audio;
using LogicWorld.Outlines;
using UnityEngine;

namespace CustomWirePlacer.Client.CWP
{
	public class CWPGroup
	{
		private PegAddress firstPeg;
		private PegAddress secondPeg;

		private IEnumerable<PegAddress> inBetween;

		private IEnumerable<PegAddress> forwards;
		private IEnumerable<PegAddress> backwards;

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
				backwards = forwards = null; //These get reset, now that their axis might have changed.
			}

			show(); //Redraw all visible outlines, respecting the skip number.
		}

		public PegAddress getSecondPeg()
		{
			return secondPeg;
		}

		public bool hasExtraPegs()
		{
			return inBetween != null || backwards != null || forwards != null;
		}

		//Should only be called when the content has changed, since quite expensive.
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

		//Ignores peg skipping, and gets used by 1-group MWP actions.
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

		public void applyGroup(CWPGroup firstGroup, PegAddress firstPeg)
		{
			//No second peg, custom code for that:
			if(firstGroup.getSecondPeg() == null)
			{
				clear();
				this.firstPeg = firstPeg;
				this.binarySkipping = firstGroup.binarySkipping;
				show();
				return; //Done.
			}

			//Second peg:
			PegAddress secondPeg = CWPHelper.getPegRelativeToOtherPeg(firstPeg, firstGroup.firstPeg, firstGroup.secondPeg);
			if(secondPeg == null)
			{
				//Cannot continue here... Don't clear data.
				SoundPlayer.PlayFail();
				return;
			}

			clear();
			this.firstPeg = firstPeg;
			this.secondPeg = secondPeg;
			this.skipNumber = firstGroup.skipNumber;
			this.binarySkipping = firstGroup.binarySkipping;
			//In between:
			this.inBetween = CWPHelper.collectPegsInBetween(firstPeg, secondPeg);
			//Backwards:
			if(firstGroup.backwards != null)
			{
				expandBackwardsInternal();
			}
			//Forwards:
			if(firstGroup.forwards != null)
			{
				expandFurtherInternal();
			}

			show();
		}

		public void switchSkipMode()
		{
			hide();
			binarySkipping = !binarySkipping;
			show();
		}
	}
}
