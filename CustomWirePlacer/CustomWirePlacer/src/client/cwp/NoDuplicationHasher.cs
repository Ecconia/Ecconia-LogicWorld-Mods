using System.Collections.Generic;

namespace CustomWirePlacer.Client.CWP
{
	public class NoDuplicationHasher<T>
	{
		private uint max;
		private uint currentID;
		private Dictionary<T, uint> dict = new Dictionary<T, uint>();
		private HashSet<uint> set = new HashSet<uint>();

		public NoDuplicationHasher(uint max)
		{
			this.max = max;
		}

		public bool probeDuplicate(T a, T b)
		{
			uint aID = getID(a);
			uint bID = getID(b);
			if(aID > bID)
			{
				(aID, bID) = (bID, aID);
			}
			uint hash = aID + bID * max;
			return set.Add(hash);
		}

		private uint getID(T obj)
		{
			if(!dict.TryGetValue(obj, out uint id))
			{
				id = currentID++;
				dict[obj] = id;
			}
			return id;
		}
	}
}
