using System;
using JimmysUnityUtilities;
using LogicWorld.SharedCode.BinaryStuff;

namespace ComponentActionExampleMod.shared.ex6
{
	// This example shows how to use the raw Component API...
	// Which means that serialization and stuff has be done manually - which is quite UFF.
	// One can just like the current custom data system, write a new framework adapter for Component API,
	// but someone has to make that first, and this is an example for Component API.
	
	public static class EditableComponentCustomData
	{
		public const byte colorAction = 1;
		public const byte heightAction = 0;
		
		public static byte[] getCustomDataFor(float height, Color24 color)
		{
			var writer = new ByteWriter();
			writer.Write(height);
			writer.Write(color);
			return writer.Finish();
		}
		
		public static (float currentHeight, Color24 currentColor) parseCustomData(byte[] dataCustomData)
		{
			var reader = new MemoryByteReader(dataCustomData);
			var height = reader.ReadFloat();
			var color = reader.ReadColor24();
			reader.Dispose();
			return (height, color);
		}
		
		public static byte[] getActionFor(float height)
		{
			var writer = new ByteWriter();
			writer.Write(heightAction);
			writer.Write(height);
			return writer.Finish();
		}
		
		public static byte[] getActionFor(Color24 color)
		{
			var writer = new ByteWriter();
			writer.Write(colorAction);
			writer.Write(color);
			return writer.Finish();
		}
		
		public static (float? height, Color24? color)? parseAction(byte[] bytes)
		{
			if(bytes.Length < 4)
			{
				return null;
			}
			var reader = new MemoryByteReader(bytes);
			var actionCode = reader.ReadByte();
			if(actionCode == colorAction)
			{
				if(bytes.Length == 4)
				{
					return (null, reader.ReadColor24());
				}
			}
			else if(actionCode == heightAction)
			{
				if(bytes.Length == 5)
				{
					return (reader.ReadFloat(), null);
				}
			}
			return null;
		}
		
		public static float extractHeight(byte[] bytes)
		{
			return BitConverter.ToSingle(bytes, 0);
		}
		
		public static Color24 extractColor(byte[] bytes)
		{
			return new Color24(bytes[4], bytes[5], bytes[6]);
		}
		
		public static void inject(float height, byte[] bytes)
		{
			var floatBytes = BitConverter.GetBytes(height);
			Array.Copy(floatBytes, 0, bytes, 0, 4);
		}
		
		public static void inject(Color24 color, byte[] bytes)
		{
			bytes[4] = color.r;
			bytes[5] = color.g;
			bytes[6] = color.b;
		}
	}
}
