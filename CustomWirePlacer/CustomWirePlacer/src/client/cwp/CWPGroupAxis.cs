using System.Collections.Generic;
using System.Linq;
using LogicAPI.Data;
using LogicWorld.Audio;
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

		private readonly List<PegAddress> blacklist = new List<PegAddress>();
		public readonly List<PegAddress> whitelist = new List<PegAddress>();

		public bool binarySkipping;
		public int skipNumber;
		public int skipOffset;

		private int pegCount;

		public void clear()
		{
			hide();
			pegCount = 0;
			firstPeg = secondPeg = null;
			inBetween = forwards = backwards = null;
			skipNumber = skipOffset = 0;
			binarySkipping = false;
			blacklist.Clear();
			whitelist.Clear();
		}

		public void hide()
		{
			CWPOutliner.RemoveOutlineHard(firstPeg);
			CWPOutliner.RemoveOutlineHard(secondPeg);
			CWPOutliner.RemoveOutlineHard(inBetween);
			CWPOutliner.RemoveOutlineHard(forwards);
			CWPOutliner.RemoveOutlineHard(backwards);
			CWPOutliner.RemoveOutlineHard(blacklist);
			CWPOutliner.RemoveOutlineHard(whitelist);
		}

		public void show()
		{
			int skipIndex = getSkipStart(); //Start with skip-number, because the first peg is always chosen.
			if(backwards != null)
			{
				for(int index = backwards.Count - 1; index >= 0; index--)
				{
					PegAddress peg = backwards[index];
					if(!blacklist.Contains(peg))
					{
						CWPOutliner.OutlineHard(peg, isNotSkipped(ref skipIndex) ? CWPOutlineData.firstDiscoveredPegs : CWPOutlineData.skippedPeg);
					}
				}
			}
			if(firstPeg != null && !blacklist.Contains(firstPeg))
			{
				CWPOutliner.OutlineHard(firstPeg, isNotSkipped(ref skipIndex) ? CWPOutlineData.firstPeg : CWPOutlineData.firstSkippedPeg);
			}
			if(inBetween != null)
			{
				foreach(PegAddress peg in inBetween)
				{
					if(!blacklist.Contains(peg))
					{
						CWPOutliner.OutlineHard(peg, isNotSkipped(ref skipIndex) ? CWPOutlineData.middlePegs : CWPOutlineData.skippedPeg);
					}
				}
			}
			if(secondPeg != null && !blacklist.Contains(secondPeg))
			{
				CWPOutliner.OutlineHard(secondPeg, isNotSkipped(ref skipIndex) ? CWPOutlineData.secondPeg : CWPOutlineData.skippedPeg);
			}
			if(forwards != null)
			{
				foreach(PegAddress peg in forwards)
				{
					if(!blacklist.Contains(peg))
					{
						CWPOutliner.OutlineHard(peg, isNotSkipped(ref skipIndex) ? CWPOutlineData.secondDiscoveredPegs : CWPOutlineData.skippedPeg);
					}
				}
			}
			foreach(PegAddress peg in blacklist)
			{
				CWPOutliner.OutlineHard(peg, CWPOutlineData.blacklistedPeg);
			}
			foreach(PegAddress peg in whitelist)
			{
				CWPOutliner.OutlineHard(peg, CWPOutlineData.whitelistedPeg);
			}
		}

		public void setSecondPeg(PegAddress secondPeg) //Nullable
		{
			hide(); //Hide all visible outlines, since some might be removed.

			this.secondPeg = secondPeg;
			inBetween = secondPeg != null ? CWPHelper.collectPegsInBetween(firstPeg, secondPeg) : null;
			backwards = forwards = null; //These get reset, now that their axis might have changed.

			show(); //Redraw all visible outlines, respecting the skip number.
		}

		public IEnumerable<PegAddress> getPegs()
		{
			int pegCount = 0;
			int skipIndex = getSkipStart(); //Start with skipNumber, to always select the first peg.
			if(backwards != null)
			{
				for(int index = backwards.Count - 1; index >= 0; index--)
				{
					PegAddress peg = backwards[index];
					if(!blacklist.Contains(peg) && isNotSkipped(ref skipIndex))
					{
						pegCount++;
						yield return peg;
					}
				}
			}
			if(!blacklist.Contains(firstPeg) && isNotSkipped(ref skipIndex))
			{
				pegCount++;
				yield return firstPeg;
			}
			if(inBetween != null)
			{
				foreach(PegAddress peg in inBetween)
				{
					if(!blacklist.Contains(peg) && isNotSkipped(ref skipIndex))
					{
						pegCount++;
						yield return peg;
					}
				}
			}
			if(secondPeg != null)
			{
				if(!blacklist.Contains(secondPeg) && isNotSkipped(ref skipIndex))
				{
					pegCount++;
					yield return secondPeg;
				}
			}
			if(forwards != null)
			{
				foreach(PegAddress peg in forwards)
				{
					if(!blacklist.Contains(peg) && isNotSkipped(ref skipIndex))
					{
						pegCount++;
						yield return peg;
					}
				}
			}
			foreach(PegAddress peg in whitelist)
			{
				pegCount++;
				yield return peg;
			}
			this.pegCount = pegCount; //Side effect, but that is fine, since there is no caching.
		}

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
			foreach(PegAddress peg in whitelist)
			{
				yield return peg;
			}
		}

		private int getSkipStart()
		{
			if(skipNumber == 0)
			{
				return binarySkipping ? 0 : 1;
			}

			if(binarySkipping)
			{
				int start = -skipOffset;
				start %= skipNumber + skipNumber;
				if(start < 0)
				{
					start += skipNumber + skipNumber;
				}
				return start;
			}
			else
			{
				int start = skipNumber - skipOffset;
				start %= skipNumber + 1;
				if(start < 0)
				{
					start += skipNumber + 1;
				}
				return start;
			}
		}

		private bool isNotSkipped(ref int skipIndex)
		{
			if(skipNumber == 0)
			{
				return true; //Skipping is not enabled.
			}

			if(binarySkipping)
			{
				bool result = skipIndex >= skipNumber;
				if(++skipIndex == skipNumber + skipNumber)
				{
					skipIndex = 0;
				}
				return result;
			}
			else
			{
				if(skipIndex++ == skipNumber)
				{
					skipIndex = 0;
					return true;
				}
				return false;
			}
		}

		public bool updateSkipNumber(int offset)
		{
			int oldValue = skipNumber;
			if(binarySkipping && CWPSettings.scrollSkipInMulDivOfTwoSteps)
			{
				roundSkipOffsetToBinary(); //Make the value valid (if required) before changing it.
				skipNumber = offset == 1 //If UP
					? skipNumber == 0
						? 1
						: skipNumber * 2
					: skipNumber / 2;
			}
			else
			{
				skipNumber += offset;
			}
			if(skipNumber < 0) //In case that we got an over or underflow, it gets captured here.
			{
				skipNumber = 0;
			}
			if(skipNumber == 0 && CWPSettings.resetSkipOffsetWhenNotSkipping)
			{
				skipOffset = 0;
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

		public bool updateSkipOffset(int offset)
		{
			if(skipNumber == 0) //If currently not skipping.
			{
				SoundPlayer.PlayFail();
				return false;
			}
			hide();
			skipOffset += offset;
			show();
			return true;
		}

		public void checkSkipOffsetReset()
		{
			if(skipOffset == 0 || skipNumber != 0)
			{
				return;
			}
			//This method has no visual side effects, the value can just be changed.
			// The reason is, that skipping is disabled anyway, so the offset has no effect.
			skipOffset = 0;
		}

		public void roundSkipOffsetToBinary(bool update = false)
		{
			if(binarySkipping && CWPSettings.roundSkipOffsetToNextBinaryNumber)
			{
				uint value = (uint) skipNumber;
				uint mask = 0x40000000; //Do not start at the very top, to prevent generating negative numbers - should not be possible, because the input is in theory non-negative.
				while(mask != 0)
				{
					if((value & mask) != 0)
					{
						uint nextBitMask = mask >> 1;
						if(nextBitMask == 0)
						{
							//We are at the lowest bit, just leave it as is (SET).
						}
						else if((value & nextBitMask) == 0 || mask == 0x40000000) //If the mask is already at the top, do not go higher.
						{
							//Rounding down!
							skipNumber = (int) mask;
						}
						else
						{
							mask <<= 1;
							skipNumber = (int) mask;
						}
						break;
					}
					mask >>= 1;
				}
				if(update)
				{
					//Only the skip number changed, the pegs are not hidden by that criteria.
					hide();
					show();
				}
			}
		}

		public void switchSkipMode()
		{
			hide();
			binarySkipping = !binarySkipping;
			roundSkipOffsetToBinary();
			show();
		}

		public void expandFurther()
		{
			if(secondPeg == null)
			{
				return;
			}
			hide();
			if(!expandFurtherInternal())
			{
				SoundPlayer.PlayFail();
			}
			show();
		}

		private bool expandFurtherInternal(bool onlyOne = false)
		{
			return expandInternal(ref forwards, firstPeg, secondPeg, inBetween?.Last(), onlyOne);
		}

		public void expandBackwards()
		{
			if(secondPeg == null)
			{
				return;
			}
			hide();
			if(!expandBackwardsInternal())
			{
				SoundPlayer.PlayFail();
			}
			show();
		}

		private bool expandBackwardsInternal(bool onlyOne = false)
		{
			return expandInternal(ref backwards, secondPeg, firstPeg, inBetween?.First(), onlyOne);
		}

		private static bool expandInternal(ref List<PegAddress> discoverList, PegAddress firstPeg, PegAddress secondPeg, PegAddress inBetweenPeg, bool onlyOne = false)
		{
			//One ray to rule them all. The ray should always be constructed from the two main pegs, it shall never bend in any other direction.
			Vector3 rayStart = CWPHelper.getRaycastPoint(firstPeg);
			Vector3 ray = (CWPHelper.getRaycastPoint(secondPeg) - rayStart).normalized; //For later calculations, it has to be normalized.

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

			//Get positions, important that the positions must be on the ray, so raycast to get the positions. 
			Vector3 pos0 = peg0 != firstPeg ? CWPHelper.getPegRayCenter(peg0, rayStart, ray) : rayStart; //If it is the first peg, the raycast would fail, so fallback. And the ray start is the first peg pos.
			Vector3 pos1 = CWPHelper.getPegRayCenter(peg1, rayStart, ray);

			//When expanding we have to find at least one peg. If there is none we cannot expand.
			PegAddress peg2 = CWPHelper.findNextPeg(pos1, ray, out Vector3? pos2nullable);
			if(peg2 == null)
			{
				return false;
			}
			Vector3 pos2 = pos2nullable.Value; //TBI: This works, but is it possible to do this syntactically more pretty?
			//The list is null when it is empty, but we found at least one peg,
			// so create the list and add that peg.
			if(discoverList == null)
			{
				discoverList = new List<PegAddress>();
			}
			discoverList.Add(peg2);
			if(onlyOne)
			{
				return true; //Expanding by mouse-wheel only requires one peg to detect.
			}

			//Now that we got one peg, there is the possibility to collect more.
			PegAddress peg3 = CWPHelper.findNextPeg(pos2, ray, out Vector3? pos3nullable);
			if(peg3 == null)
			{
				//However there is no more peg, so there is no need to expand from here on.
				return true;
			}
			Vector3 pos3 = pos3nullable.Value;

			//Calculate all the distances between the pegs, required to decide for an expand strategy.
			float dist1 = (pos1 - pos0).sqrMagnitude;
			float dist2 = (pos2 - pos1).sqrMagnitude;
			float dist3 = (pos3 - pos2).sqrMagnitude;

			float referenceDistance;
			if(CWPSettings.expandOnlyUniformDistance)
			{
				//If the uniform distance setting is ON, then only the original distance will be used as reference.
				// So expanding only expands if the distance is the same as the original distance.
				Vector3 previousPos = inBetweenPeg != null ? CWPHelper.getPegRayCenter(inBetweenPeg, rayStart, ray) : rayStart; //Ray start is the first pos point.
				referenceDistance = (CWPHelper.getRaycastPoint(secondPeg) - previousPos).sqrMagnitude;
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
			return true;
		}

		//When this method gets called, we are trying to figure out if the provided peg
		// should be added to the current section, or be not added, because it belongs to the next section.
		//To do this, we are comparing its distance to the next peg (if exists), and see if it is closer to that one.
		private static void loopExpand(
			ICollection<PegAddress> discoverList, Vector3 ray, float distance,
			float dist0, Vector3 pos0, PegAddress peg0
		)
		{
			int pegCounter = 0;
			//If the reference distance, is not the same as the distance to this peg,
			// The section is 100% changed, thus stop collecting pegs here.
			while(isSame(distance, dist0))
			{
				//Get the next peg, to compare the distances:
				PegAddress peg1 = CWPHelper.findNextPeg(pos0, ray, out Vector3? pos1nullable);
				if(peg1 == null)
				{
					//No next peg, then the peg must belong to the active section.
					discoverList.Add(peg0);
					return;
				}
				Vector3 pos1 = pos1nullable.Value;
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
				if(pegCounter++ > 1000)
				{
					return; //Okay this is enough for now. The client might freeze for too long.
				}
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
				applyLists(otherAxis);
				show();
				return; //Done.
			}

			//Second peg:
			this.secondPeg = secondPeg;
			clear();
			this.firstPeg = firstPeg;
			this.secondPeg = secondPeg;
			this.skipNumber = otherAxis.skipNumber;
			this.skipOffset = otherAxis.skipOffset;
			this.binarySkipping = otherAxis.binarySkipping;
			//In between:
			this.inBetween = CWPHelper.collectPegsInBetween(firstPeg, secondPeg);
			//Backwards:
			if(otherAxis.backwards != null)
			{
				for(int i = 0; i < otherAxis.backwards.Count; i++)
				{
					if(!expandBackwardsInternal(true))
					{
						break;
					}
				}
			}
			//Forwards:
			if(otherAxis.forwards != null)
			{
				for(int i = 0; i < otherAxis.forwards.Count; i++)
				{
					if(!expandFurtherInternal(true))
					{
						break;
					}
				}
			}
			applyLists(otherAxis);

			show();
		}

		private void applyLists(CWPGroupAxis otherAxis)
		{
			foreach(PegAddress peg in otherAxis.blacklist)
			{
				PegAddress other = CWPHelper.getPegRelativeToOtherPeg(firstPeg, otherAxis.firstPeg, peg);
				if(other != null)
				{
					blacklist.Add(other);
				}
			}
			foreach(PegAddress peg in otherAxis.whitelist)
			{
				PegAddress other = CWPHelper.getPegRelativeToOtherPeg(firstPeg, otherAxis.firstPeg, peg);
				if(other != null)
				{
					whitelist.Add(other);
				}
			}
		}

		public void updateExpandBackwardsCount(int offset)
		{
			hide();
			if(offset == 1)
			{
				expandBackwardsInternal(true);
			}
			else if(backwards != null)
			{
				backwards.RemoveAt(backwards.Count - 1);
				if(!backwards.Any())
				{
					backwards = null;
				}
			}
			else
			{
				SoundPlayer.PlayFail();
			}
			show();
		}

		public void updateExpandFurtherCount(int offset)
		{
			hide();
			if(offset == 1)
			{
				expandFurtherInternal(true);
			}
			else if(forwards != null)
			{
				forwards.RemoveAt(forwards.Count - 1);
				if(!forwards.Any())
				{
					forwards = null;
				}
			}
			else
			{
				SoundPlayer.PlayFail();
			}
			show();
		}

		public void toggleList(PegAddress peg)
		{
			hide();

			if(blacklist.Contains(peg))
			{
				blacklist.Remove(peg);
			}
			else if(whitelist.Contains(peg))
			{
				whitelist.Remove(peg);
			}
			else if(isPartOfMainAxis(peg))
			{
				blacklist.Add(peg);
			}
			else
			{
				whitelist.Add(peg);
			}

			show();
		}

		private bool isPartOfMainAxis(PegAddress peg)
		{
			return firstPeg == peg
				|| secondPeg == peg
				|| (inBetween != null && inBetween.Contains(peg))
				|| (forwards != null && forwards.Contains(peg))
				|| (backwards != null && backwards.Contains(peg));
		}

		public int getPegCount()
		{
			return pegCount;
		}
	}
}
