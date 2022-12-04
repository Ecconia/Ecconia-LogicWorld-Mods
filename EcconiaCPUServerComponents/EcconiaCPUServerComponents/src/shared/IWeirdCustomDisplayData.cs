namespace EcconiaCPUServerComponents.Shared
{
	public interface IWeirdCustomDisplayData
	{
		//for 128 bytes
		//32 by 32 pixels
		//4 bytes per row.
		//Order:
		// X first 0...31
		// Y second 0...31
		//MSB is lower pixel index
		//LSB is higher pixel index
		byte[] pixelData { get; set; }
	}
}
