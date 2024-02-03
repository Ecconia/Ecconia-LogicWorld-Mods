using EcconiaCPUServerComponents.Shared;
using JimmysUnityUtilities;
using LogicWorld.Server.Circuitry;

namespace EcconiaCPUServerComponents.Server
{
	public class FlatKey : LogicComponent<IFlatKeyData>
	{
		protected override void OnCustomDataUpdated()
		{
			//When custom data was changed, there is a high chance, that the state changed.
			// So the output must update.
			QueueLogicUpdate();
		}
		
		protected override void DoLogicUpdate()
		{
			//Update the output, with whatever the state says.
			//Might be needless in some cases, but that does not matter.
			this.Outputs[0].On = this.Data.KeyDown;
		}
		
		protected override void SetDataDefaultValues()
		{
			//Not that this matters...
			Data.KeyDown = false;
			Data.BoundInput = 2;
			Data.KeyColor = new Color24(85, 85, 85);
			Data.KeyLabelColor = new Color24(229, 229, 229);
			Data.sizeX = 1;
			Data.sizeZ = 1;
		}
	}
}
