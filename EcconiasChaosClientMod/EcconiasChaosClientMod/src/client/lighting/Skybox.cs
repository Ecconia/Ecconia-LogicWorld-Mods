using LICC;
using LogicAPI;
using LogicLog;
using UnityEngine;

namespace EcconiasChaosClientMod.Client.Lighting
{
	public static class Skybox
	{
		private static Texture2D texture;
		private static IModFiles files;
		
		public static void init(IModFiles files, ILogicLogger logger)
		{
			Skybox.files = files;
			var skyboxFile = findSkyboxTexture();
			if(skyboxFile == null)
			{
				logger.Info("Did not find any skybox matching: 'skyboxes/*.png' in the mod files.");
				return;
			}
			var data = skyboxFile.ReadAllBytes();
			texture = new Texture2D(1, 1);
			texture.LoadImage(data, false);
			logger.Info("Loaded skybox with: " + texture.width + " | " + texture.height);
			texture = FlipTexture(texture);
		}
		
		static Texture2D FlipTexture(Texture2D original)
		{
			var flipped = new Texture2D(original.width, original.height);
			var width = original.width;
			var height = original.height;
			for(var x = 0; x < width; x++)
			{
				for(var y = 0; y < height; y++)
				{
					flipped.SetPixel(x, height - y - 1, original.GetPixel(x, y));
				}
			}
			flipped.Apply();
			return flipped;
		}
		
		private static ModFile findSkyboxTexture()
		{
			foreach(var file in files.EnumerateFiles())
			{
				if(file.Extension.Equals(".png") && file.Path.StartsWith("skyboxes/"))
				{
					return file;
				}
			}
			return null;
		}
		
		[Command(name: "Skybox", Description = "Activates a custom skybox from the skybox folder (if any). Read the readme (inside that folder) to learn more.")]
		public static void skybox()
		{
			if(texture == null)
			{
				LConsole.WriteLine("You must place at least one PNG file into the 'skyboxes' folder of this mod. Read its readme file.");
				return;
			}
			skyboxSilent();
		}
		
		public static void skyboxSilent()
		{
			if(texture == null)
			{
				return;
			}
			var skybox = RenderSettings.skybox;
			skybox.SetTexture("_Tex", CubemapFromTexture2D(texture));
			DynamicGI.UpdateEnvironment();
		}
		
		public static Cubemap CubemapFromTexture2D(Texture2D texture)
		{
			var side = texture.width / 4;
			var cube = new Cubemap(side, TextureFormat.ARGB32, false);
			var up = texture.GetPixels(1 * side, 0 * side, side, side);
			var down = texture.GetPixels(1 * side, 2 * side, side, side);
			var left = texture.GetPixels(0 * side, 1 * side, side, side);
			var forward = texture.GetPixels(1 * side, 1 * side, side, side);
			var right = texture.GetPixels(2 * side, 1 * side, side, side);
			var back = texture.GetPixels(3 * side, 1 * side, side, side);
			cube.SetPixels(up, CubemapFace.PositiveY);
			cube.SetPixels(down, CubemapFace.NegativeY);
			cube.SetPixels(forward, CubemapFace.PositiveZ);
			cube.SetPixels(back, CubemapFace.NegativeZ);
			cube.SetPixels(left, CubemapFace.NegativeX);
			cube.SetPixels(right, CubemapFace.PositiveX);
			cube.Apply();
			return cube;
		}
	}
}
