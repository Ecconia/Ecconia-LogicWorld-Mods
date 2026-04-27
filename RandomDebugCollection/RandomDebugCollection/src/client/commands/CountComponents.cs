using System.Collections.Generic;
using LICC;
using LogicAPI;
using LogicAPI.Data;
using LogicWorld.Interfaces;
using LogicWorld.Physics;
using LogicWorld.Players;

namespace RandomDebugCollection.Client.Commands
{
	public class CountComponents
	{
		[Command]
		public static void countComponents()
		{
			var lookingAt = PlayerCaster.CameraCast(Masks.Default | Masks.Environment | Masks.Structure | Masks.Peg | Masks.PlayerModel);
			if (!lookingAt.HitComponent)
			{
				return;
			}
			var componentAddress = lookingAt.cAddress;
			
			var counts = new Dictionary<ComponentType, int>();
			
			{
				var component = Instances.MainWorld.Data.Lookup(componentAddress);
				var type = component.Data.Type;
				var old = counts.GetValueOrDefault(type, 0);
				counts[type] = old + 1;
			}
			
			foreach (var subAddress in Instances.MainWorld.Data.EnumerateComponentsInTree(componentAddress))
			{
				var component = Instances.MainWorld.Data.Lookup(subAddress);
				var type = component.Data.Type;
				var old = counts.GetValueOrDefault(type, 0);
				counts[type] = old + 1;
			}
			
			LConsole.WriteLine("Counts:");
			foreach (var (type, count) in counts)
			{
				LConsole.WriteLine("- " + Instances.MainWorld.ComponentTypes.GetTextID(type) + ": " + count);
			}
		}
	}
}
