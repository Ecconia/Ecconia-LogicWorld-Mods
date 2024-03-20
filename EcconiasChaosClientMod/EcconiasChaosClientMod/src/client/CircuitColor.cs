using System.Collections.Generic;
using EccsLogicWorldAPI.Shared.AccessHelper;
using JimmysUnityUtilities;
using LICC;
using LogicAPI.Data;
using LogicWorld.Interfaces;
using LogicWorld.SharedCode;
using LogicWorld.SharedCode.Components;
using LogicWorld.UI.Thumbnails;

namespace EcconiasChaosClientMod.Client
{
	public static class CircuitColor
	{
		[Command("SetCircuitColor", Description = "Sets the color of circuit components. Might not fully apply, with active building operations. Does not apply to rendered component thumbnails.")]
		private static void setCircuitColor(byte r, byte g, byte b)
		{
			var color = new Color24(r, g, b);
			setCircuitColor(color, true);
		}
		
		public static void setCircuitColor(Color24 color, bool updateThumbnails = false)
		{
			var gpuColor = color.ToGpuColor();
			// Will set the color for new and switching components:
			setColorInternally(color);
			if(Instances.MainWorld == null)
			{
				return; // No world to update.
			}
			
			// Update all existing components:
			updateExistingEntities(gpuColor);
			
			// Update component thumbnails:
			var rendererInstance = Types.checkType<ItemThumbnails>(Fields.getNonNull(Fields.getPrivateStatic(typeof(ItemThumbnails), "Instance")));
			var method = Methods.getPrivate(typeof(ItemThumbnails), "QueueRender");
			
			var affectedComponentTypes = new HashSet<string>();
			foreach(var type in ComponentRegistry.GetAllTextIDs())
			{
				var componentInfo = ComponentRegistry.GetComponentInfoByTextID(type);
				if (!componentInfo.PrefabIsDynamic)
				{
					Prefab staticPrefab = componentInfo.StaticPrefab;
					foreach(var outputs in staticPrefab.Outputs)
					{
						if(outputs.StartOn)
						{
							affectedComponentTypes.Add(type);
							
							var hotbarItem = new BasicHotbarItemData(type);
							var texture = ItemThumbnails.GetThumbnailFor(hotbarItem);
							method.Invoke(rendererInstance, new object[] {hotbarItem, texture});
							
							goto end_of_loop;
						}
					}
				}
				end_of_loop: {}
			}
			
			// Update thumbnails in hotbar...
			var hotbar = Instances.Hotbar;
			for(int i = 0; i < Instances.Hotbar.HotbarItemsCount; i++)
			{
				var hotbarItem = hotbar.HotbarItemInfo(i) as DetailedHotbarItemData;
				if(hotbarItem == null)
				{
					continue; // Wrong hotbar item type.
				}
				if(!affectedComponentTypes.Contains(hotbarItem.TextID))
				{
					continue; // No startOn peg.
				}
				// Also re-render that:
				var texture = ItemThumbnails.GetThumbnailFor(hotbarItem);
				method.Invoke(rendererInstance, new object[] {hotbarItem, texture});
			}
		}
		
		private static void updateExistingEntities(GpuColor color)
		{
			var entityTracker = Instances.MainWorld.Renderer.Entities;
			var circuitStates = Instances.MainWorld.CircuitStates;
			foreach(var (address, wire) in Instances.MainWorld.Data.AllWires)
			{
				if(circuitStates.GetStateAt(wire.StateID))
				{
					var entity = entityTracker.GetWireEntity(address);
					entity.SetColor(color);
				}
			}
			foreach(var (address, componentDataManager) in Instances.MainWorld.Data.AllComponents)
			{
				int index = 0;
				foreach(var inputInfo in componentDataManager.Data.InputInfos)
				{
					if(circuitStates.GetStateAt(inputInfo.StateID))
					{
						var entity = entityTracker.GetPegEntity(new PegAddress(address, index, PegType.Input));
						entity.SetColor(color);
					}
					index += 1;
				}
				index = 0;
				foreach(var outputInfo in componentDataManager.Data.OutputInfos)
				{
					if(circuitStates.GetStateAt(outputInfo.StateID))
					{
						var entity = entityTracker.GetPegEntity(new PegAddress(address, index, PegType.Output));
						entity.SetColor(color);
					}
					index += 1;
				}
			}
		}
		
		private static void setColorInternally(Color24 color)
		{
			var colorOn24Field = Fields.getPublicStatic(typeof(Colors), nameof(Colors.CircuitOn24));
			var colorOnField = Fields.getPublicStatic(typeof(Colors), nameof(Colors.CircuitOn));
			colorOn24Field.SetValue(null, color);
			colorOnField.SetValue(null, color.ToGpuColor());
		}
	}
}
