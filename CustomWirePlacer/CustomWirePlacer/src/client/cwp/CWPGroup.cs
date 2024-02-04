using System.Collections.Generic;
using System.Linq;
using LogicAPI.Data;
using LogicWorld.Audio;
using UnityEngine;

namespace CustomWirePlacer.Client.CWP
{
	public class CWPGroup
	{
		private readonly CWPGroupAxis firstAxis = new CWPGroupAxis();
		private readonly CWPGroupAxis secondAxis = new CWPGroupAxis();
		
		private CWPGroupAxis currentAxis;
		
		private readonly List<PegAddress> pegs2DOutlined = new List<PegAddress>();
		private readonly List<PegAddress> pegs2D = new List<PegAddress>();
		
		public CWPGroup()
		{
			//Hmm why does C# not allow this in the field section?
			currentAxis = firstAxis;
		}
		
		public void clear()
		{
			currentAxis = firstAxis;
			firstAxis.clear();
			secondAxis.clear();
			pegs2D.Clear();
			hideInternal();
			pegs2DOutlined.Clear();
		}
		
		private void hideInternal()
		{
			foreach(var peg in pegs2DOutlined)
			{
				CWPOutliner.RemoveOutlineHard(peg);
			}
		}
		
		private void updated2D()
		{
			if(secondAxis.firstPeg.IsEmpty())
			{
				return; //Nothing to do.
			}
			
			pegs2D.Clear();
			hideInternal();
			pegs2DOutlined.Clear();
			
			//Collecting the pegs is redundant here...
			var skipHighlightPegs = secondAxis.getAllPegs().ToHashSet();
			
			var secondOffsets = get2DOffsets();
			foreach(var startingPeg in firstAxis.getPegs())
			{
				foreach(var receivedPeg in get2DPegs(startingPeg, secondOffsets))
				{
					pegs2D.Add(receivedPeg);
					if(!skipHighlightPegs.Contains(receivedPeg))
					{
						pegs2DOutlined.Add(receivedPeg);
					}
				}
			}
			
			showInternal();
		}
		
		public List<Vector3> get2DOffsets()
		{
			//Get the second axis and all the offsets in between pegs:
			var secondAxisList = secondAxis.getPegs().ToList();
			var secondOffsets = new List<Vector3>(secondAxisList.Count - 1);
			{
				var lastPos = CWPHelper.getWireConnectionPoint(secondAxisList.First());
				secondOffsets.Add(lastPos - CWPHelper.getWireConnectionPoint(secondAxis.firstPeg));
				for(var i = 1; i < secondAxisList.Count; i++)
				{
					var nextPos = CWPHelper.getWireConnectionPoint(secondAxisList[i]);
					secondOffsets.Add(nextPos - lastPos);
					lastPos = nextPos;
				}
			}
			return secondOffsets;
		}
		
		public static IEnumerable<PegAddress> get2DPegs(PegAddress startingPeg, List<Vector3> secondOffsets)
		{
			//Get the starting point, from which we stack up, according to the second axis.
			var offset = CWPHelper.getWireConnectionPoint(startingPeg);
			foreach(var secondOffset in secondOffsets)
			{
				offset += secondOffset;
				var receivedPeg = CWPHelper.getPegAt(offset);
				if(receivedPeg.IsNotEmpty())
				{
					yield return receivedPeg;
				}
			}
		}
		
		private void showInternal()
		{
			foreach(var peg in pegs2DOutlined)
			{
				CWPOutliner.OutlineHard(peg, CWPOutlineData.middlePegs);
			}
		}
		
		public void bakePegOutlines()
		{
			if(isTwoDimensional())
			{
				//Remove the already backed pegs of the first axis:
				foreach(var peg in firstAxis.getPegs())
				{
					CWPOutliner.RemoveOutlineSoft(peg);
				}
				CWPOutliner.RemoveOutlineSoft(firstAxis.firstPeg);
				//And remove all hard outlines:
				hideInternal();
				secondAxis.hide();
			}
			else
			{
				firstAxis.hide();
			}
			foreach(var peg in getPegs())
			{
				CWPOutliner.OutlineSoft(peg, CWPOutlineData.backedGroupOne);
			}
			CWPOutliner.OutlineSoft(firstAxis.firstPeg, CWPOutlineData.backedGroupOneFirst, true);
		}
		
		public bool isSet()
		{
			return firstAxis.firstPeg.IsNotEmpty();
		}
		
		public void setFirstPeg(PegAddress firstPeg, bool outline = true)
		{
			//If dirty call, handle anyway:
			if(firstAxis.firstPeg.IsNotEmpty())
			{
				//Well... lets just clear it - should not have happened.
				clear();
			}
			
			//Expecting this to only happen once after clearing.
			firstAxis.firstPeg = firstPeg;
			
			//Outline the first peg:
			if(outline)
			{
				CWPOutliner.OutlineHard(firstPeg, CWPOutlineData.firstPeg);
			}
		}
		
		public void showFirstPeg()
		{
			CWPOutliner.OutlineHard(firstAxis.firstPeg, CWPOutlineData.firstPeg);
			firstAxis.getPegs(); //Fix the peg count.
		}
		
		public PegAddress getRootPeg()
		{
			return firstAxis.firstPeg;
		}
		
		public void setSecondPeg(PegAddress secondPeg) //Nullable
		{
			currentAxis.setSecondPeg(secondPeg);
			updated2D();
		}
		
		public PegAddress getSecondPeg()
		{
			return currentAxis.secondPeg;
		}
		
		//This is only called when there is one 1D group. 
		public bool hasExtraPegs()
		{
			return firstAxis.inBetween != null || firstAxis.backwards != null || firstAxis.forwards != null || firstAxis.whitelist.Any();
		}
		
		public bool isNotSWP()
		{
			return isTwoDimensional() || firstAxis.backwards != null || firstAxis.forwards != null || firstAxis.whitelist.Any() || firstAxis.blacklist.Any() || firstAxis.skipNumber != 0;
		}
		
		//Should only be called when the content has changed, since quite expensive.
		public List<PegAddress> getPegs()
		{
			return isTwoDimensional() ? pegs2D : currentAxis.getPegs();
		}
		
		public bool updateSkipNumber(int value)
		{
			bool result = currentAxis.updateSkipNumber(value);
			updated2D();
			return result;
		}
		
		public bool updateSkipOffset(int offset)
		{
			bool result = currentAxis.updateSkipOffset(offset);
			updated2D();
			return result;
		}
		
		public void switchSkipMode()
		{
			currentAxis.switchSkipMode();
			updated2D();
		}
		
		public void expandFurther()
		{
			currentAxis.expandFurther();
			updated2D();
		}
		
		public void expandBackwards()
		{
			currentAxis.expandBackwards();
			updated2D();
		}
		
		public void updateExpandBackwardsCount(int offset)
		{
			currentAxis.updateExpandBackwardsCount(offset);
			updated2D();
		}
		
		public void updateExpandFurtherCount(int offset)
		{
			currentAxis.updateExpandFurtherCount(offset);
			updated2D();
		}
		
		public bool applyGroup(CWPGroup firstGroup, PegAddress firstPeg)
		{
			//Check if the other starting peg can be found:
			// Else no data should be erased.
			var otherPeg = PegAddress.Empty;
			var otherAxisSecondPeg = PegAddress.Empty;
			if(firstGroup.isTwoDimensional())
			{
				otherPeg = CWPHelper.getPegRelativeToOtherPeg(firstPeg, firstGroup.firstAxis.firstPeg, firstGroup.secondAxis.firstPeg);
				if(otherPeg.IsEmpty())
				{
					SoundPlayer.PlayFail();
					return false;
				}
				if(firstGroup.secondAxis.secondPeg.IsNotEmpty())
				{
					otherAxisSecondPeg = CWPHelper.getPegRelativeToOtherPeg(firstPeg, firstGroup.firstAxis.firstPeg, firstGroup.secondAxis.secondPeg);
					if(otherAxisSecondPeg.IsEmpty())
					{
						SoundPlayer.PlayFail();
						return false;
					}
				}
			}
			var firstGroupSecondPeg = PegAddress.Empty;
			if(firstGroup.firstAxis.secondPeg.IsNotEmpty())
			{
				firstGroupSecondPeg = CWPHelper.getPegRelativeToOtherPeg(firstPeg, firstGroup.firstAxis.firstPeg, firstGroup.firstAxis.secondPeg);
				if(firstGroupSecondPeg.IsEmpty())
				{
					SoundPlayer.PlayFail();
					return false;
				}
			}
			
			//Apply the axes:
			firstAxis.applyAxis(firstGroup.firstAxis, firstPeg, firstGroupSecondPeg);
			if(firstGroup.isTwoDimensional())
			{
				secondAxis.applyAxis(firstGroup.secondAxis, otherPeg, otherAxisSecondPeg);
			}
			
			updated2D();
			return true;
		}
		
		public void startTwoDimensional(PegAddress startingPeg)
		{
			currentAxis = secondAxis;
			currentAxis.firstPeg = startingPeg;
			currentAxis.show();
			updated2D();
		}
		
		public bool isTwoDimensional()
		{
			return secondAxis.firstPeg.IsNotEmpty();
		}
		
		public PegAddress getFirstPeg()
		{
			return currentAxis.firstPeg;
		}
		
		public CWPGroupAxis getFirstAxis()
		{
			return firstAxis;
		}
		
		public CWPGroupAxis getSecondAxis()
		{
			return secondAxis;
		}
		
		public CWPGroupAxis getCurrentAxis()
		{
			return currentAxis;
		}
		
		public void toggleList(PegAddress peg)
		{
			currentAxis.toggleList(peg);
			updated2D();
		}
		
		public bool hasMultiplePegs()
		{
			return firstAxis.secondPeg.IsNotEmpty() || firstAxis.whitelist.Any() || secondAxis.firstPeg.IsNotEmpty();
		}
		
		public int getPegCount()
		{
			return isTwoDimensional() ? pegs2D.Count : firstAxis.getPegCount();
		}
	}
}
