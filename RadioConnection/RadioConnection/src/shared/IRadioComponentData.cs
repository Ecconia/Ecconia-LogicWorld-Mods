using LICC;

namespace RadioConnection.Shared
{
	public interface IRadioComponentData
	{
		uint addressBase { get; set; }
		uint addressPegs { get; set; }
		uint dataOffset { get; set; }
		uint dataPegs { get; set; }
		bool useLinkLayer { get; set; }
		bool compactPegPlacement { get; set; }
		bool flipped { get; set; }
	}
	
	public static class Initialize
	{
		public static void initialize(this IRadioComponentData data)
		{
			data.dataOffset = 0;
			data.dataPegs = 4;
			data.addressBase = 0;
			data.addressPegs = 0;
			data.useLinkLayer = false;
			data.compactPegPlacement = false;
			data.flipped = false;
		}
		
		public static void debug(this IRadioComponentData data)
		{
			LConsole.WriteLine("Data: " + data.addressPegs + " / " + data.dataPegs + " : " + data.addressBase + " | " + data.dataOffset + " : " + data.useLinkLayer + " | " + data.compactPegPlacement + " | " + data.flipped);
		}
	}
}
