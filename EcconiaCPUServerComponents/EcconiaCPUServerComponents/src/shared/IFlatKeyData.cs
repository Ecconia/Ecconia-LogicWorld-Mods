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
	
	public static class FlatKeyDataExtension
	{
		public static void initialize(this IFlatKeyData data)
		{
			data.KeyDown = false;
			data.BoundInput = 2;
			data.KeyColor = new Color24(85, 85, 85);
			data.KeyLabelColor = new Color24(229, 229, 229);
			data.sizeX = 1;
			data.sizeZ = 1;
			data.label = null; //Yes 'null' is the default - means no overwrite.
		}
	}
}
