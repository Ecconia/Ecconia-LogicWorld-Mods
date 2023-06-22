using System.Reflection;
using LICC;
using LogicUI.Palettes;

namespace RandomDebugCollection.Client.Commands
{
	public static class ReloadCurrentTheme
	{
		[Command("theme", Description = "Reload the current theme from disk")]
		public static void reloadCurrentTheme()
		{
			var f = typeof(PaletteManager).GetProperty("PaletteSetting", BindingFlags.Static | BindingFlags.NonPublic);
			if(f == null)
			{
				LConsole.WriteLine("Whoops cannot find property containing the palette name...");
				return;
			}
			var name = f.GetValue(null);
			if(name == null || name.GetType() != typeof(string))
			{
				LConsole.WriteLine("Whoops the palette name was either 'null' or not a string...");
				return;
			}
			PaletteManager.LoadPaletteFromGameData((string) name);
		}
	}
}
