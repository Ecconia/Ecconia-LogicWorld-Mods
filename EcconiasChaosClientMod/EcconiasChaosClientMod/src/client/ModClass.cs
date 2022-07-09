using System;
using LICC;
using LogicAPI;
using LogicAPI.Client;
using UnityEngine;

namespace EcconiasChaosClientMod.Client
{
	public class ModClass : ClientMod
	{
		private static Texture2D texture;

		protected override void Initialize()
		{
			ModFile skyboxFile = findSkyboxTexture();
			if(skyboxFile == null)
			{
				Logger.Info("Did not find any skybox matching: 'skyboxes/*.png' in the mod files.");
				return;
			}
			byte[] data = skyboxFile.ReadAllBytes();
			texture = new Texture2D(1, 1);
			texture.LoadImage(data, false);
			Logger.Info("Loaded skybox with: " + texture.width + " | " + texture.height);
			texture = FlipTexture(texture);
		}
		
		Texture2D FlipTexture(Texture2D original)
		{
			Texture2D flipped = new Texture2D(original.width, original.height);
			int width = original.width;
			int height = original.height;
			for(int x = 0; x < width; x++)
			{
				for(int y = 0; y < height; y++)
				{
					flipped.SetPixel(x, height - y - 1, original.GetPixel(x, y));
				}
			}
			flipped.Apply();
			return flipped;
		}

		private ModFile findSkyboxTexture()
		{
			foreach(var file in Files.EnumerateFiles())
			{
				if(file.Extension.Equals(".png") && file.Path.StartsWith("skyboxes/"))
				{
					return file;
				}
			}
			return null;
		}

		[Command(name: "Skybox")]
		public static void skybox()
		{
			if(texture == null)
			{
				LConsole.WriteLine("You must place at least one PNG file into the 'skyboxes' folder of this mod. Read its readme file.");
				return;
			}
			Material skybox = RenderSettings.skybox;
			skybox.SetTexture("_Tex", CubemapFromTexture2D(texture));
			DynamicGI.UpdateEnvironment();
		}

		public static Cubemap CubemapFromTexture2D(Texture2D texture)
		{
			int side = texture.width / 4;
			Cubemap cube = new Cubemap(side, TextureFormat.ARGB32, false);
			Color[] up = texture.GetPixels(1 * side, 0 * side, side, side);
			Color[] down = texture.GetPixels(1 * side, 2 * side, side, side);
			Color[] left = texture.GetPixels(0 * side, 1 * side, side, side);
			Color[] forward = texture.GetPixels(1 * side, 1 * side, side, side);
			Color[] right = texture.GetPixels(2 * side, 1 * side, side, side);
			Color[] back = texture.GetPixels(3 * side, 1 * side, side, side);
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
