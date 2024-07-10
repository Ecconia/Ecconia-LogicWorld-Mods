using FancyInput;
using LogicAPI.Client;
using PrimitiveSelections.Client.Inputs;

namespace PrimitiveSelections.Client
{
	public class PrimitiveSelections : ClientMod
	{
		protected override void Initialize()
		{
			CustomInput.Register<PrimitiveSelectionsContext, PrimitiveSelectionsTriggers>("PrimitiveSelections"); 
		}
	}
}
