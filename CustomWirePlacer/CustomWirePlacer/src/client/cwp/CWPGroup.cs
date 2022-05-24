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

		public void hide()
		{
			firstAxis.hide();
			secondAxis.hide();
			hideInternal();
		}

		private void hideInternal()
		{
			foreach(PegAddress peg in pegs2DOutlined)
			{
				Outliner.RemoveHardOutline(peg);
			}
		}

		private void updated2D()
		{
			if(secondAxis.firstPeg == null)
			{
				return; //Nothing to do.
			}

			pegs2D.Clear();
			hideInternal();
			pegs2DOutlined.Clear();

			//Collecting the pegs is redundant here...
			HashSet<PegAddress> skipHighlightPegs = firstAxis.getAllPegs().Concat(secondAxis.getAllPegs()).ToHashSet();

			List<Vector3> secondOffsets = get2DOffsets();
			foreach(PegAddress startingPeg in firstAxis.getPegs())
			{
				foreach(PegAddress receivedPeg in get2DPegs(startingPeg, secondOffsets))
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
			List<PegAddress> secondAxisList = secondAxis.getPegs().ToList();
			List<Vector3> secondOffsets = new List<Vector3>(secondAxisList.Count - 1);
			{
				Vector3 lastPos = CWPHelper.getWireConnectionPoint(secondAxisList.First());
				secondOffsets.Add(lastPos - CWPHelper.getWireConnectionPoint(secondAxis.firstPeg));
				for(int i = 1; i < secondAxisList.Count; i++)
				{
					Vector3 nextPos = CWPHelper.getWireConnectionPoint(secondAxisList[i]);
					secondOffsets.Add(nextPos - lastPos);
					lastPos = nextPos;
				}
			}
			return secondOffsets;
		}

		public static IEnumerable<PegAddress> get2DPegs(PegAddress startingPeg, List<Vector3> secondOffsets)
		{
			//Get the starting point, from which we stack up, according to the second axis.
			Vector3 offset = CWPHelper.getWireConnectionPoint(startingPeg);
			foreach(Vector3 secondOffset in secondOffsets)
			{
				offset += secondOffset;
				PegAddress receivedPeg = CWPHelper.getPegAt(offset);
				if(receivedPeg != null)
				{
					yield return receivedPeg;
				}
			}
		}

		public void show()
		{
			firstAxis.show();
			secondAxis.show();
			showInternal();
		}

		private void showInternal()
		{
			foreach(PegAddress peg in pegs2DOutlined)
			{
				Outliner.HardOutline(peg, CWPOutlineData.middlePegs);
			}
		}

		public bool isSet()
		{
			return firstAxis.firstPeg != null;
		}

		public void setFirstPeg(PegAddress firstPeg)
		{
			//If dirty call, handle anyway:
			if(firstAxis.firstPeg != null)
			{
				//Well... lets just clear it - should not have happened.
				clear();
			}

			//Expecting this to only happen once after clearing.
			firstAxis.firstPeg = firstPeg;

			//Outline the first peg:
			Outliner.HardOutline(firstPeg, CWPOutlineData.firstPeg);
		}

		public PegAddress getFirstPeg()
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
			return firstAxis.inBetween != null || firstAxis.backwards != null || firstAxis.forwards != null;
		}

		//Should only be called when the content has changed, since quite expensive.
		public IEnumerable<PegAddress> getPegs()
		{
			return isTwoDimensional() ? pegs2D : currentAxis.getPegs();
		}

		//Ignores peg skipping, and gets used by 1-group MWP actions.
		public IEnumerable<PegAddress> getAllPegs()
		{
			return currentAxis.getAllPegs();
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

		public void applyGroup(CWPGroup firstGroup, PegAddress firstPeg)
		{
			//Check if the other starting peg can be found:
			// Else no data should be erased.
			PegAddress otherPeg = null;
			PegAddress otherAxisSecondPeg = null;
			if(firstGroup.isTwoDimensional())
			{
				otherPeg = CWPHelper.getPegRelativeToOtherPeg(firstPeg, firstGroup.firstAxis.firstPeg, firstGroup.secondAxis.firstPeg);
				if(otherPeg == null)
				{
					SoundPlayer.PlayFail();
					return;
				}
				otherAxisSecondPeg = CWPHelper.getPegRelativeToOtherPeg(firstPeg, firstGroup.secondAxis.firstPeg, firstGroup.secondAxis.secondPeg);
				if(otherAxisSecondPeg == null)
				{
					SoundPlayer.PlayFail();
					return;
				}
			}
			PegAddress firstGroupSecondPeg = CWPHelper.getPegRelativeToOtherPeg(firstPeg, firstGroup.firstAxis.firstPeg, firstGroup.firstAxis.secondPeg);
			if(firstGroupSecondPeg == null)
			{
				SoundPlayer.PlayFail();
				return;
			}

			//Apply the axes:
			firstAxis.applyAxis(firstGroup.firstAxis, firstPeg, firstGroupSecondPeg);
			if(firstGroup.isTwoDimensional())
			{
				secondAxis.applyAxis(firstGroup.secondAxis, otherPeg, otherAxisSecondPeg);
			}

			updated2D();
		}

		public void startTwoDimensional(PegAddress startingPeg)
		{
			currentAxis = secondAxis;
			currentAxis.firstPeg = startingPeg;
			currentAxis.show();
		}

		public bool isTwoDimensional()
		{
			return secondAxis.firstPeg != null;
		}

		public PegAddress getStartPeg()
		{
			return currentAxis.firstPeg;
		}

		public CWPGroupAxis getFirstAxis()
		{
			return firstAxis;
		}

		public CWPGroupAxis getCurrentAxis()
		{
			return currentAxis;
		}
	}
}
