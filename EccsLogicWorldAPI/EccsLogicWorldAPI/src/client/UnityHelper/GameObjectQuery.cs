using UnityEngine;

namespace EccsLogicWorldAPI.Client.UnityHelper
{
	public static class GameObjectQuery
	{
		public static GameObject queryGameObject(params string[] arguments)
		{
			if(arguments.Length == 0)
			{
				return null;
			}
			
			foreach(var obj in Resources.FindObjectsOfTypeAll<GameObject>())
			{
				if(obj.transform.parent != null)
				{
					continue; //Not a root level object, can be discarded.
				}
				if(!arguments[0].Equals(obj.name))
				{
					continue;
				}
				return arguments.Length == 1 ? obj : queryGameObject(obj, 1, arguments);
			}
			return null;
		}
		
		public static GameObject queryGameObject(GameObject gameObject, params string[] arguments)
		{
			if(arguments.Length == 0)
			{
				return null;
			}
			
			return queryGameObject(gameObject, 0, arguments);
		}
		
		//Internal:
		
		private static GameObject queryGameObject(GameObject gameObject, int index, string[] arguments)
		{
			string argument = arguments[index++];
			foreach(var transChild in gameObject.GetComponentsInChildren<Transform>(true))
			{
				var obj = transChild.gameObject;
				if(transChild.parent != gameObject.transform || !argument.Equals(obj.name))
				{
					continue;
				}
				return arguments.Length == index ? obj : queryGameObject(obj, index, arguments);
			}
			return null;
		}
	}
}
