using JimmysUnityUtilities;

namespace EcconiaCPUServerComponents.Shared
{
	public interface IFlatKeyData
	{
		bool KeyDown { get; set; }
		int BoundInput { get; set; }

		Color24 KeyColor { get; set; }
		Color24 KeyLabelColor { get; set; }
		
		int sizeX { get; set; }
		int sizeZ { get; set; }
		
		string label { get; set; }
	}
}
