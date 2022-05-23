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

		public List<PegAddress> forwards;
		public List<PegAddress> backwards;

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
				for(int index = backwards.Count - 1; index >= 0; index--)
				{
					Outliner.HardOutline(backwards[index], isNotSkipped(ref skipIndex) ? CWPOutlineData.firstDiscoveredPegs : CWPOutlineData.skippedPeg);
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
				for(int index = backwards.Count - 1; index >= 0; index--)
				{
					if(isNotSkipped(ref skipIndex))
					{
						yield return backwards[index];
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
				for(int index = backwards.Count - 1; index >= 0; index--)
				{
					yield return backwards[index];
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
			expandInternal(ref forwards, firstPeg, secondPeg, inBetween?.Last());
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
			expandInternal(ref backwards, secondPeg, firstPeg, inBetween?.First());
		}

		private static void expandInternal(ref List<PegAddress> discoverList, PegAddress firstPeg, PegAddress secondPeg, PegAddress inBetweenPeg)
		{
			//This is the peg, which is before the last peg in expand direction.
			// Required to get the distance between this one and the actual last peg.
			PegAddress peg0 =
				discoverList != null
					? discoverList.Count > 1
						? discoverList[discoverList.Count - 2]
						: secondPeg
					: inBetweenPeg != null
						? inBetweenPeg
						: firstPeg;
			//This is the last peg in this expand direction.
			// From here we expand.
			PegAddress peg1 =
				discoverList != null
					? discoverList.Last()
					: secondPeg;

			//Get positions and calculate the ray, which may have any length:
			Vector3 pos0 = CWPHelper.getWireConnectionPoint(peg0);
			Vector3 pos1 = CWPHelper.getWireConnectionPoint(peg1);
			Vector3 ray = pos1 - pos0;

			//When expanding we have to find at least one peg. If there is none we cannot expand.
			PegAddress peg2 = CWPHelper.findNextPeg(pos1, ray);
			if(peg2 == null)
			{
				SoundPlayer.PlayFail();
				return;
			}
			//The list is null when it is empty, but we found at least one peg,
			// so create the list and add that peg.
			if(discoverList == null)
			{
				discoverList = new List<PegAddress>();
			}
			discoverList.Add(peg2);
			
			Vector3 pos2 = CWPHelper.getWireConnectionPoint(peg2);

			//Now that we got one peg, there is the possibility to collect more.
			PegAddress peg3 = CWPHelper.findNextPeg(pos2, ray);
			if(peg3 == null)
			{
				//However there is no more peg, so there is no need to expand from here on.
				return;
			}
			Vector3 pos3 = CWPHelper.getWireConnectionPoint(peg3);
			
			//Calculate all the distances between the pegs, required to decide for an expand strategy.
			float dist1 = (pos1 - pos0).sqrMagnitude;
			float dist2 = (pos2 - pos1).sqrMagnitude;
			float dist3 = (pos3 - pos2).sqrMagnitude;
			
			float referenceDistance;
			if(CWPSettings.expandOnlyUniformDistance)
			{
				//If the uniform distance setting is ON, then only the original distance will be used as reference.
				// So expanding only expands if the distance is the same as the original distance.
				PegAddress previousPeg = inBetweenPeg != null ? inBetweenPeg : firstPeg;
				referenceDistance = (CWPHelper.getWireConnectionPoint(secondPeg) - CWPHelper.getWireConnectionPoint(previousPeg)).sqrMagnitude;
			}
			else
			{
				//Expansion works the same, a peg (here the third peg), is being investigated, if it is part of the current section
				// or part of a new section, which we do not care about.
				//The reference distance, tells how big the distance of the current section shall be.
				// With that, it collects all pegs that have the same distance and are not closer to other sections.
				//There are only two distances to consider, the distance of the old section (dist1), or
				// the distance of a new section (dist3).
				//We choose the old sections distance over the new sections distance, if:
				// The distance between the old existing pegs and the first discovered is the same
				//  AND the distance between the two new pegs is NOT smaller than the old sections distance.
				referenceDistance = isSame(dist1, dist2) && (dist3 > dist1 || isSame(dist1, dist3)) ? dist1 : dist3;
			}
			loopExpand(
				discoverList, ray, referenceDistance,
				dist3, pos3, peg3
			);
		}

		//When this method gets called, we are trying to figure out if the provided peg
		// should be added to the current section, or be not added, because it belongs to the next section.
		//To do this, we are comparing its distance to the next peg (if exists), and see if it is closer to that one.
		private static void loopExpand(
			ICollection<PegAddress> discoverList, Vector3 ray, float distance,
			float dist0, Vector3 pos0, PegAddress peg0
		)
		{
			//If the reference distance, is not the same as the distance to this peg,
			// The section is 100% changed, thus stop collecting pegs here.
			while(isSame(distance, dist0))
			{
				//Get the next peg, to compare the distances:
				PegAddress peg1 = CWPHelper.findNextPeg(pos0, ray);
				if(peg1 == null)
				{
					//No next peg, then the peg must belong to the active section.
					discoverList.Add(peg0);
					return;
				}
				Vector3 pos1 = CWPHelper.getWireConnectionPoint(peg1);
				float dist1 = (pos1 - pos0).sqrMagnitude;

				//If the distance of the probe peg to the current peg is smaller than the previous distance,
				// the current peg must belong to a new section and collecting pegs should stop.
				if(!(distance < dist1 || isSame(distance, dist1)))
				{
					return;
				}
				discoverList.Add(peg0);

				//Start the next loop with the probe peg as the current peg:
				dist0 = dist1;
				pos0 = pos1;
				peg0 = peg1;
			}
		}

		private static bool isSame(float a, float b)
		{
			return Mathf.Abs(a - b) < 0.0001f;
		}

		public void applyAxis(CWPGroupAxis otherAxis, PegAddress firstPeg, PegAddress secondPeg)
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
			this.secondPeg = secondPeg;
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
