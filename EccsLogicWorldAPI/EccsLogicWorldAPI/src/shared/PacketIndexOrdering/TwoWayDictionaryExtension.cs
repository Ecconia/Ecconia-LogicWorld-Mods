using System;
using System.Collections.Generic;
using JimmysUnityUtilities.Collections;

namespace EccsLogicWorldAPI.Shared.PacketIndexOrdering
{
	public static class TwoWayDictionaryExtension
	{
		public static TwoWayDictionary<ushort, Type> Clone(this TwoWayDictionary<ushort, Type> original)
		{
			return new TwoWayDictionary<ushort, Type>(new Dictionary<Type, ushort>(original.Backwards));
		}
		
		public static bool Remove(this TwoWayDictionary<ushort, Type> original, Type toRemove, out ushort index)
		{
			if(original.TryGetValue(toRemove, out index))
			{
				original.Remove(toRemove);
				return true;
			}
			
			index = 0;
			return false;
		}
		
		public static bool Remove(this TwoWayDictionary<ushort, Type> original, ushort toRemove, out Type type)
		{
			if(original.TryGetValue(toRemove, out type))
			{
				original.Remove(toRemove);
				return true;
			}
			
			type = null;
			return false;
		}
		
		public static bool TryAdd(this TwoWayDictionary<ushort, Type> original, ushort index, Type type)
		{
			if(original.ContainsKey(index) || original.ContainsKey(type))
			{
				return false;
			}
			original.Add(index, type);
			return true;
		}
	}
}
