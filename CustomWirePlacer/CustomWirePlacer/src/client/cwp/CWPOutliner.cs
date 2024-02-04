using System.Collections.Generic;
using LogicAPI.Data;
using LogicWorld.Interfaces;
using LogicWorld.Outlines;

namespace CustomWirePlacer.Client.CWP
{
	public static class CWPOutliner
	{
		private static readonly Dictionary<PegAddress, PegData> pegDatas = new Dictionary<PegAddress, PegData>();
		
		private static PegData getOrInject(PegAddress peg)
		{
			if(!pegDatas.TryGetValue(peg, out var data))
			{
				data = new PegData()
				{
					entity = Instances.MainWorld.Renderer.Entities.GetPegEntity(peg),
				};
				pegDatas.Add(peg, data);
			}
			return data;
		}
		
		public static void OutlineHard(PegAddress peg, OutlineData color)
		{
			PegData data = getOrInject(peg);
			Outliner.Outline(data.entity, color, true); //Always set the outline/color for hard outlines.
			data.hardCounter += 1;
		}
		
		public static void OutlineSoft(PegAddress peg, OutlineData color, bool forceSet = false)
		{
			var data = getOrInject(peg);
			if(data.softCounter == 0 || forceSet) //When there is no color, this was just created. Else this already has a color, do not overwrite that color - unless force.
			{
				data.color = color;
				if(data.hardCounter == 0) //Only actually add/update the outline, when there is no hard outline active.
				{
					Outliner.Outline(data.entity, color, true);
				}
			}
			data.softCounter += 1;
		}
		
		public static void RemoveOutlineHard(PegAddress peg)
		{
			if(peg.IsEmpty())
			{
				return;
			}
			if(pegDatas.TryGetValue(peg, out var data))
			{
				if(data.hardCounter <= 0)
				{
					//Ehm, there is no hard outline to remove.
					return;
				}
				data.hardCounter -= 1;
				if(data.hardCounter == 0)
				{
					if(data.softCounter > 0)
					{
						if(data.color.HasValue) //Should have a color at this point...
						{
							Outliner.Outline(data.entity, data.color.Value); //Update color
						}
					}
					else //No more outline left on this peg. Remove fully.
					{
						Outliner.RemoveOutline(data.entity);
						pegDatas.Remove(peg);
					}
				}
			}
		}
		
		public static void RemoveOutlineHard(IEnumerable<PegAddress> pegs)
		{
			if(pegs == null)
			{
				return;
			}
			foreach(var peg in pegs)
			{
				RemoveOutlineHard(peg);
			}
		}
		
		public static void RemoveOutlineSoft(PegAddress peg)
		{
			if(peg.IsEmpty())
			{
				return;
			}
			if(pegDatas.TryGetValue(peg, out var data))
			{
				if(data.softCounter <= 0)
				{
					return; //Ehm, there was no soft outline for this peg to remove.
				}
				data.softCounter -= 1;
				if(data.hardCounter <= 0 && data.softCounter <= 0)
				{
					Outliner.RemoveOutline(data.entity);
					pegDatas.Remove(peg);
				}
			}
		}
		
		public static void RemoveAllOutlines()
		{
			foreach(var data in pegDatas.Values)
			{
				Outliner.RemoveOutline(data.entity);
			}
			pegDatas.Clear();
		}
		
		private class PegData
		{
			public IRenderedEntity entity;
			public OutlineData? color;
			public int softCounter;
			public int hardCounter;
		}
	}
}
