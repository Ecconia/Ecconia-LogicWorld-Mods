using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LICC;
using LogicUI.Palettes;

namespace RandomDebugCollection.Client.Commands
{
	public static class ReloadCurrentTheme
	{
		private static readonly Func<string> getCurrentTheme;
		
		static ReloadCurrentTheme()
		{
			getCurrentTheme = Delegator.createStaticPropertyGetter<string>(Properties.getPrivateStatic(typeof(PaletteManager), "PaletteSetting"));
		}
		
		[Command("Theme", Description = "Reload the current theme from disk")]
		public static void reloadCurrentTheme()
		{
			var name = getCurrentTheme();
			if(name == null)
			{
				LConsole.WriteLine("Whoops the palette name was 'null'...");
				return;
			}
			PaletteManager.LoadPaletteFromGameData(name);
		}
	}
}
