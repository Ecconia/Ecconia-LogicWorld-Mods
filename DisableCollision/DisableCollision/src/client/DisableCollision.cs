//Framework imports:
using LogicAPI.Client; //Needed for 'ClientMod'
using HarmonyLib; //Needed for 'Harmony'
using LICC; //Needed for 'LConsole' and 'Command'

//Custom imports:
using LogicWorld.Building; //Needed for 'Intersections' and 'WireUtility'

namespace DisableCollision.Client
{
	public class DisableCollision : ClientMod
	{
		//This flag indicates if the collision should be ignored or not
		public static bool enable; //Needs to be public, for patches to access.
		//This flag gets set if the warning has been displayed
		private static bool showedWarning = false;
		
		//Entry point of this mod
		protected override void Initialize()
		{
			//Sets up Harmony and applies the patches
			var harmony = new Harmony("DisableCollision");
			harmony.PatchAll();
		}
		
		//Command of this mod, in charge of toggling, if this mod should be active or not
		[Command("ToggleCollision", Description = "Disables collision when building! Mod by Ecconia. Only use this mod when the game gives you a hard time, do not intentionally clip components and wires into each other.")]
		private static void ToggleClipping()
		{
			//Toggle activeness and print a message for new state
			enable = !enable;
			if(enable)
			{
				//On first enable, print the disclaimer warning
				if(showedWarning == false)
				{
					showedWarning = true;
					LConsole.WriteLine("ATTENTION!", CColor.Red);
					LConsole.WriteLine("This mod was written to allow building when Logic World thinks there is collision, but there is actually none. Also when you want to refactor or take apart a building which would break wires.", CColor.Red);
					LConsole.WriteLine("Do NOT use it to intentionally clip components or wires into each other! This is not the intended use case for this mod.", CColor.Red);
					LConsole.WriteLine("If you however ABUSE this mod in an unintended way, do NOT confuse others with it. And CREDIT the usage of this mod, so that others especially the developers know, that the creation was not done by normal means.", CColor.Red);
				}
				LConsole.WriteLine("Building with collision ENABLED!");
			}
			else
			{
				LConsole.WriteLine("Building with collision DISABLED!");
			}
		}
	}
	
	[HarmonyPatch(typeof(Intersections))]
	[HarmonyPatch("CollidersIntersectingSomething")]
	public class PatchOne
	{
		public static bool Prefix(ref bool __result)
		{
			//Only if active, we want to change the behavior, else just do nothing
			if(DisableCollision.enable)
			{
				//False here stands for "there is no clipping"
				__result = false;
				//Skips the execution of the original method
				return false;
			}
			//Else: Proceed with the original implementation
			return true;
		}
	}
	
	[HarmonyPatch(typeof(WireUtility))]
	[HarmonyPatch("ValidWireLineOfSight")]
	public class PatchTwo
	{
		public static bool Prefix(ref bool __result)
		{
			//Only if active, we want to change the behavior, else just do nothing
			if(DisableCollision.enable)
			{
				//True here stands for "there is a line of sight"
				__result = true;
				//Skips the execution of the original method
				return false;
			}
			//Else: Proceed with the original implementation
			return true;
		}
	}
}
