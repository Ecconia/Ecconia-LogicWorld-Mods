using LICC;
using UnityEngine;

namespace EcconiasChaosClientMod.Client.Lighting
{
	public class LightingCommands
	{
		[Command(name: "LightAmbient", Description = "Sets the color of the ambient light, with equal distribution over RGB channel.")]
		public static void lightAmbient(float intensity)
		{
			RenderSettings.ambientSkyColor = new Color(intensity, intensity, intensity);
		}
		
		[Command(name: "LightSun", Description = "Sets the color of the sun light, with equal distribution over RGB channel.")]
		public static void lightSun(float intensity)
		{
			RenderSettings.sun.color = new Color(intensity, intensity, intensity);
		}
		
		[Command(name: "LightPreset", Description = "Activates the skybox and some preset values, that you can change in code.")]
		public static void lightPreset()
		{
			Skybox.skyboxSilent();
			lightSun(0.5f);
			lightAmbient(0.9f);
		}
	}
}
