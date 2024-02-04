using System.Text;
using LICC;
using LogicAPI.Data;
using LogicWorld.Interfaces;
using LogicWorld.Networking;
using LogicWorld.Players;
using UnityEngine;
using Random = System.Random;

namespace EcconiasChaosClientMod.Client
{
	public static class RainbowChat
	{
		[Command(name: "RainbowChat", Description = "Will print its text-argument in chat with rainbow colors <#FF00DD>R</color><#FF0091>E</color><#FF0044>G</color><#FF0800>E</color><#FF5500>N</color><#FFA100>B</color><#FFEE00>O</color><#C4FF00>G</color><#77FF00>E</color><#2BFF00>N</color>")]
		public static void chatRainbow(string text)
		{
			var random = new Random();
			var currentHue = random.Next(256) / 256.0f;
			var offsetVariance = 0.08f * random.Next(256) / 256.0f - 0.04f;
			var builder = new StringBuilder();
			var advance = text.Length < 8 ? 0.1f : 0.05f + offsetVariance;
			foreach(var c in text)
			{
				if(c == ' ')
				{
					//Skip for space.
					builder.Append(' ');
					continue;
				}
				builder.Append("<#").Append(h(ref currentHue)).Append('>').Append(c).Append("</color>");
			}
			Instances.SendData.ChatMessage(new ChatMessageData()
			{
				Sender = CurrentPlayer.Username,
				Color = AvatarSettings.BodyColor,
				MessageContent = builder.ToString(),
			});
			
			string h(ref float hue)
			{
				var aa = Color.HSVToRGB(hue, 1.0f, 1.0f);
				hue += advance;
				if(hue > 1)
				{
					hue -= 1;
				}
				return ColorUtility.ToHtmlStringRGB(aa);
			}
		}
	}
}
