using System;
using System.Collections.Generic;
using System.Reflection;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicWorld.GameStates;

namespace EccsLogicWorldAPI.Client.Injectors
{
	public static class GameStateInjector
	{
		public static void inject(string identifier, Type newGameState)
		{
			if(!typeof(GameState).IsAssignableFrom(newGameState))
			{
				throw new Exception("Provided game state type is not actually a game state? Got: " + newGameState);
			}
			FieldInfo field = Fields.getPrivateStatic(typeof(GameStateManager), "GameStateInstancesByTextID");
			var gameStateDictionary = Types.checkType<IDictionary<string, GameState>>(Fields.getNonNull(field));
			if(!gameStateDictionary.ContainsKey(identifier))
			{
				gameStateDictionary[identifier] = (GameState) Activator.CreateInstance(newGameState);
			}
		}
	}
}
