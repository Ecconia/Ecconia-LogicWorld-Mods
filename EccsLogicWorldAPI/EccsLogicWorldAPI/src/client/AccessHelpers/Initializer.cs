using System;
using LogicInitializable;
using LogicWorld;
using UnityEngine;

namespace EccsLogicWorldAPI.Client.AccessHelpers
{
	public static class Initializer
	{
		//And the fancy way to initialize anything with a hammer...
		public static void recursivelyInitialize(GameObject gameObject, bool triggerErrorScreen = true)
		{
			try
			{
				foreach(var component in gameObject.GetComponents<IInitializable>())
				{
					component.Initialize();
				}
				foreach(Transform child in gameObject.transform)
				{
					recursivelyInitialize(child.gameObject);
				}
			}
			catch(Exception e)
			{
				if(triggerErrorScreen)
				{
					SceneAndNetworkManager.TriggerErrorScreen(e);
				}
				else
				{
					throw;
				}
			}
		}
	}
}
