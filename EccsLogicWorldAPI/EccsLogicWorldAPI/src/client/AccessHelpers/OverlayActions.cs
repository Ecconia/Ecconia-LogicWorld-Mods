using System;
using System.Linq.Expressions;
using System.Reflection;
using LogicWorld.GameStates;

namespace EccsLogicWorldAPI.Client.AccessHelpers
{
	public static class OverlayActions
	{
		//These two functions allow access to the game state overlay actions:
		// Uses reflection, since both are set to private.
		
		public static Action getOverlayShownAction()
		{
			var method = typeof(GameStateManager).GetMethod("OverlayAdded", BindingFlags.Static | BindingFlags.NonPublic);
			if(method == null)
			{
				throw new Exception("Could not find method 'OverlayAdded' in 'GameStateManager'.");
			}
			return Expression.Lambda<Action>(Expression.Call(method)).Compile();
		}
		
		public static Action getOverlayHidingAction()
		{
			var method = typeof(GameStateManager).GetMethod("OverlayRemoved", BindingFlags.Static | BindingFlags.NonPublic);
			if(method == null)
			{
				throw new Exception("Could not find method 'OverlayRemoved' in 'GameStateManager'.");
			}
			return Expression.Lambda<Action>(Expression.Call(method)).Compile();
		}
	}
}
