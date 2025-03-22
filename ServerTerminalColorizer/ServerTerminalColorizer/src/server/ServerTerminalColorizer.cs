using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;
using LICC;
using LICC.API;
using LogicAPI.Server;
using LogicAPI.Server.Configuration;
using LogicWorld.Server;

namespace ServerTerminalColorizer.Server
{
	public class ServerTerminalColorizer : ServerMod
	{
		protected override void Initialize()
		{
			// Do not apply this feature on internal servers. As the LW client terminal can display rich-text but not ANSI.
			var launchOptions = Program.Get<LaunchOptions>();
			if(launchOptions == null)
			{
				throw new Exception("Could not get LunchOptions.");
			}
			if(launchOptions.Integrated)
			{
				return;
			}
			
			// Apply patch:
			var methodToHijack = typeof(Frontend).GetMethod(nameof(Frontend.WriteLine), BindingFlags.Instance | BindingFlags.Public, [ typeof(string) ]);
			var methodHook = typeof(ServerTerminalColorizer).GetMethod(nameof(hook), BindingFlags.Static | BindingFlags.NonPublic);
			var harmony = new Harmony("ServerTerminalColorConverter");
			harmony.Patch(methodToHijack, prefix: new HarmonyMethod(methodHook));
		}
		
		/*
		 * Do some replacement magic!
		 * Supported are:
		 * - </color>
		 * - <color=#abc>
		 * - <color=#abcdef>
		 * - <#abc>
		 * - <#abcdef>
		 * But not:
		 * - <color=colorname>
		 */
		
		private static void hook(ref string str)
		{
			if(str.IndexOf('<') < 0)
			{
				return;
			}
			
			str = str.Replace("</color>", "\e[m");
			str = Regex.Replace(
				str,
				"<(?:color=)?#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})>",
				match => {
					var hex = match.Groups[1].Value;
					// Expand the hex code to 6 digits:
					if (hex.Length == 3)
					{
						hex = $"{hex[0]}0{hex[1]}0{hex[2]}0";
					}
					var color = new CColor(int.Parse(hex, NumberStyles.HexNumber));
					return color.ToAnsiRGB();
				},
				RegexOptions.Multiline | RegexOptions.Compiled
			);
		}
	}
}
