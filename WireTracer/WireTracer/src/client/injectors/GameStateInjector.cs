using System;
using System.Collections.Generic;
using System.Reflection;
using LogicLog;
using LogicWorld.GameStates;

namespace WireTracer.Client.injectors
{
	public static class GameStateInjector
	{
		public static bool inject(ILogicLogger logger, string identifier, Type newGameState)
		{
			if(!typeof(GameState).IsAssignableFrom(newGameState))
			{
				logger.Error("Provided game state type is not actually a game state? Got: " + newGameState);
				return false;
			}
			FieldInfo field = typeof(GameStateManager).GetField("GameStateInstancesByTextID", BindingFlags.NonPublic | BindingFlags.Static);
			if(field == null)
			{
				logger.Error("Could not find field 'GameStateInstancesByTextID' in 'GameStateManager'.");
				return false;
			}
			object fieldValue = field.GetValue(null);
			if(fieldValue == null)
			{
				logger.Error("Value of field 'GameStateInstancesByTextID' in 'GameStateManager' was 'null'.");
				return false;
			}
			if(!(fieldValue is IDictionary<string, GameState>))
			{
				logger.Error("Value of field 'GameStateInstancesByTextID' in 'GameStateManager' was not of type 'IDictionary<string, GameState>', but: " + fieldValue.GetType());
				return false;
			}
			var gameStateDictionary = fieldValue as IDictionary<string, GameState>;
			if(!gameStateDictionary.ContainsKey(identifier))
			{
				gameStateDictionary[identifier] = (GameState) Activator.CreateInstance(newGameState);
			}
			return true;
		}
	}
}
