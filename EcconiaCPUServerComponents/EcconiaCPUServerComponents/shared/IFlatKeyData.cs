using LogicWorld.SharedCode.ComponentCustomData;

namespace EcconiaCPUServerComponents.Shared
{
	public interface IFlatKeyData : IKeyData
	{
		int sizeX { get; set; }
		int sizeZ { get; set; }
		
		string label { get; set; }
	}
}
