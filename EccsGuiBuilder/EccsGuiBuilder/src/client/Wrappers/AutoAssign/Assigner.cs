using System;
using System.Collections.Generic;
using System.Reflection;
using EccsLogicWorldAPI.Shared;
using EccsLogicWorldAPI.Shared.AccessHelper;
using UnityEngine;

namespace EccsGuiBuilder.Client.Wrappers.AutoAssign
{
	public static class Assigner
	{
		public static void assign(Wrapper contentWrapper, GameObject root)
		{
			//Collect keys:
			var keys = new List<(string, GameObject, Func<GameObject, GameObject>)>();
			contentWrapper.collectAllInjectionKeys(keys);
			var resolvedKeys = new Dictionary<string, GameObject>();
			keys.ForEach(entry => {
				var gameObject = entry.Item2;
				NullChecker.check(gameObject, "Named Injection Key's gameObject was null, this should never happen!");
				try
				{
					var resolvedGameObject = entry.Item3(gameObject);
					resolvedKeys.Add(entry.Item1, resolvedGameObject);
				}
				catch(Exception e)
				{
					throw new Exception("Named Injection Keys resolver failed to resolve GameObject. Details in wrapped exception.", e);
				}
			});
			
			var classes = new List<IAssignMyFields>();
			root.GetComponents(classes);
			foreach(var obj in classes)
			{
				foreach(var field in obj.GetType().GetFields(Bindings.ppInst))
				{
					var attribute = (AssignMe[]) field.GetCustomAttributes(typeof(AssignMe));
					if(attribute.Length != 0)
					{
						//Only one is allowed, so lets get the attribute:
						var key = attribute[0].key;
						if(key == null)
						{
							key = field.Name;
						}
						resolvedKeys.TryGetValue(key, out GameObject gameObject);
						NullChecker.check(gameObject, $"Could not find key '{attribute[0].key}' defined in field: {obj.GetType().Name}");
						var targetType = field.FieldType;
						if(targetType == typeof(GameObject))
						{
							field.SetValue(obj, gameObject);
							continue;
						}
						if(!targetType.IsSubclassOf(typeof(Component)))
						{
							throw new Exception($"Field {field.Name} in class {obj.GetType().Name} tries to receive injections for a non Unity-Component/GameObject type. Only Unity-Components/GameObjects can be injected.");
						}
						var actualComponent = gameObject.GetComponent(targetType);
						if(actualComponent == null)
						{
							actualComponent = gameObject.GetComponentInChildren(targetType);
							NullChecker.check(actualComponent, $"Could not find type '{targetType}' on game-object with name {gameObject.name}");
						}
						//Proceed with actual injection:
						field.SetValue(obj, actualComponent);
					}
				}
			}
		}
	}
}
