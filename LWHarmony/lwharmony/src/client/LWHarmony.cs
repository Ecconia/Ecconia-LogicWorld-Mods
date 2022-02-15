using System.Reflection;

using LogicAPI.Client;
using LogicLog;

namespace LWHarmony
{
	public class Injector: ClientMod
	{
		protected override void Initialize()
		{
			foreach (var file in Files.EnumerateFiles())
			{
				if (file.Path.EndsWith(".dll"))
				{
					Logger.Info(string.Concat("Loading ", file.FileName));
					DllUtil.Loader.Load(file);
				}
			}
		}
	}
}
