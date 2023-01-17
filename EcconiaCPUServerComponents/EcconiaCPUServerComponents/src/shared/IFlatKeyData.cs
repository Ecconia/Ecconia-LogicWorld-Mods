using JimmysUnityUtilities;

namespace EcconiaCPUServerComponents.Shared
{
	//TODO: Find another way, to make LogicWorld aware of that this key is "just like a normal key" (or copy its UI completely).
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
