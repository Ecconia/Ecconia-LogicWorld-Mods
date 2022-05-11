using System.Collections.Generic;
using LogicAPI.Data;
using LogicWorld.Outlines;

namespace CustomWirePlacer.Client.CWP
{
	public class CWPGroup
	{
		private CWPGroupAxis firstAxis = new CWPGroupAxis();

		private CWPGroupAxis currentAxis;

		public CWPGroup()
		{
			//Hmm why does C# not allow this in the field section?
			currentAxis = firstAxis;
		}
		
		public void clear()
		{
			firstAxis.clear();
		}

		public void hide()
		{
			firstAxis.hide();
		}

		public void show()
		{
			firstAxis.show();
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
		}

		public PegAddress getSecondPeg()
		{
			//TODO: Dangerous when 2D, this has to change.
			return currentAxis.secondPeg;
		}

		public bool hasExtraPegs()
		{
			return firstAxis.inBetween != null || firstAxis.backwards != null || firstAxis.forwards != null;
		}

		//Should only be called when the content has changed, since quite expensive.
		public IEnumerable<PegAddress> getPegs()
		{
			return currentAxis.getPegs();
		}

		//Ignores peg skipping, and gets used by 1-group MWP actions.
		public IEnumerable<PegAddress> getAllPegs()
		{
			return currentAxis.getAllPegs();
		}

		public bool updateSkipNumber(int value)
		{
			return currentAxis.updateSkipNumber(value);
		}
		
		public void switchSkipMode()
		{
			currentAxis.switchSkipMode();
		}

		public void expandFurther()
		{
			currentAxis.expandFurther();
		}

		public void expandBackwards()
		{
			currentAxis.expandBackwards();
		}

		public void applyGroup(CWPGroup firstGroup, PegAddress firstPeg)
		{
			firstAxis.applyAxis(firstGroup.firstAxis, firstPeg);
			//TODO: Second axis.
		}
	}
}
