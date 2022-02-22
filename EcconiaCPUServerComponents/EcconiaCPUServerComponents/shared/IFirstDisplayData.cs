namespace EcconiaCPUServerComponents.Shared
{
	public interface IFirstDisplayData
	{
		//for 128 bytes
		//32 by 32 pixels
		//4 bytes per row.
		//Order:
		// X first 0...31
		// Y second 0...31
		//LSB is lower pixel index
		//MSB is higher pixel index
		byte[] pixelData { get; set; }
	}
}
