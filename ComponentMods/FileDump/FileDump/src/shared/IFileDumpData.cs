namespace FileDump.Shared
{
	public interface IFileDumpData
	{
		uint pegCount { get; set; }
		string fileName { get; set; }
	}
	
	public static class Initialize
	{
		public static void initialize(this IFileDumpData data)
		{
			data.pegCount = 4;
			data.fileName = "FileDump.txt";
		}
	}
}
