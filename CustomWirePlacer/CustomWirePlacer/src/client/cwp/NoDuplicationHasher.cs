using System.Collections.Generic;

namespace CustomWirePlacer.Client.CWP
{
	public class NoDuplicationHasher<T>
	{
		private readonly uint max;
		
		private readonly Dictionary<T, uint> dict = new Dictionary<T, uint>();
		private readonly HashSet<uint> set = new HashSet<uint>();
		
		private uint currentID;
		
		public NoDuplicationHasher(uint max)
		{
			this.max = max;
		}
		
		public bool probeDuplicate(T a, T b)
		{
			var aID = getID(a);
			var bID = getID(b);
			if(aID > bID)
			{
				(aID, bID) = (bID, aID);
			}
			var hash = aID + bID * max;
			return set.Add(hash);
		}
		
		private uint getID(T obj)
		{
			if(!dict.TryGetValue(obj, out var id))
			{
				id = currentID++;
				dict[obj] = id;
			}
			return id;
		}
	}
}
