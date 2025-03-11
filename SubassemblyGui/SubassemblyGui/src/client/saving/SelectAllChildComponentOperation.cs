using FancyInput;
using LogicWorld.Building.Overhaul;
using SubassemblyGui.Client.Inputs;

namespace SubassemblyGui.Client.Saving
{
	public class SaveSubassemblyOperation : BuildingOperation
	{
		public override string IconHexCode => "f0c7";
		
		public override bool CanOperateOn(ComponentSelection selection)
		{
			return true;
		}
		
		public override void BeginOperationOn(ComponentSelection selection)
		{
			SavingGui.startWithSelection(selection);
		}
		
		public override InputTrigger OperationStarter => SubassemblyGuiTriggers.SaveSubassemblyGui;
	}
}
