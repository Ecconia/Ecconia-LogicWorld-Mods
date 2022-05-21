using System;
using System.Collections.Generic;
using System.Reflection;
using LogicUI.Palettes;
using UnityEngine;

namespace EccsWindowHelper.Client.Experimental
{
	/// <summary>
	/// Class <c>SerializeFieldInjector</c> is a tool, used to automatically inject Components into fields.
	/// Guided injection is possible too. It is written as a tool for the development process.
	/// Once the development process is over, this should no longer be used.
	/// </summary>
	public static class SerializeFieldInjector
	{
		public static void injectInto(Component obj, HashSet<string> ignoreKeys, Dictionary<string, object> dictionary = null)
		{
			injectInto(obj, dictionary, ignoreKeys);
		}

		public static void injectInto(Component obj, Dictionary<string, object> dictionary = null, HashSet<string> ignoreKeys = null)
		{
			ModClass.logger.Info("Processing: " + obj.GetType().Name);
			foreach(var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				if(field.GetCustomAttribute<SerializeField>() == null)
				{
					ModClass.logger.Info(" Skipping: " + field.Name);
					continue;
				}
				if(ignoreKeys != null && ignoreKeys.Contains(field.Name))
				{
					ModClass.logger.Info(" Skipping: " + field.Name + " (custom)");
					continue;
				}

				Type targetType = field.FieldType;
				if(dictionary != null && dictionary.ContainsKey(field.Name))
				{
					ModClass.logger.Info(" Injecting into '" + field.Name + "' type '" + targetType.Name + "' (custom)");
					field.SetValue(obj, dictionary[field.Name]);
					continue;
				}

				if(targetType == typeof(PaletteData) || targetType == typeof(PaletteColor))
				{
					//Skip this one, because it requires a normal setter.
					ModClass.logger.Info(" Skipping: " + field.Name + ", call 'SetPaletteColor()' instead.");
					continue;
				}

				ModClass.logger.Info(" Injecting into '" + field.Name + "' type '" + targetType.Name + "'");

				if(targetType.IsSubclassOf(typeof(Component)))
				{
					Component comp = obj.gameObject.GetComponent(targetType);
					if(comp == null)
					{
						ModClass.logger.Info("  FAIL, no such component registered.");
						continue;
					}
					field.SetValue(obj, comp);
				}
				else
				{
					ModClass.logger.Info("  FAIL, cannot get this type yet.");
				}
			}
		}
	}
}
