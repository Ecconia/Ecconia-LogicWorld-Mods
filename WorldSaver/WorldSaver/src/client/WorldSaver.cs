using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LICC;
using LogicAPI.Client;
using LogicWorld.Interfaces;
using LogicWorld.Rendering;
using LogicWorld.SharedCode.ExtraData;
using LogicWorld.SharedCode.Saving;
using LogicWorld.SharedCode.WorldTypes;
using SUCC;
using UnityEngine.SceneManagement;

namespace WorldSaver.Client
{
	public class WorldSaver : ClientMod
	{
		protected override void Initialize()
		{
		}

		[Command("SaveWorld", Description = "Yeah don't, if you don't know what you are doing.")]
		public static void saveWorld()
		{
			//Ensure target folder exists:
			var savesFolder = Path.Combine(SUCC.Utilities.DefaultPath, "saves");
			var saveFolder = Path.Combine(savesFolder, "WorldExport");
			if(Directory.Exists(saveFolder))
			{
				LConsole.WriteLine("First rename the 'WorldExport' folder.");
				return;
			}

			//Get world bytes to export:
			bool[] circuitStates = extractCircuitStates();
			if(circuitStates == null)
			{
				return;
			}
			byte[] saveBytes = SaveWriter.GetWorldSaveData(
				Instances.MainWorld.Data,
				Instances.MainWorld.ComponentTypes,
				circuitStates
			);

			//Save world file:
			Directory.CreateDirectory(saveFolder);
			//Save to world file:
			File.WriteAllBytes(Path.Combine(saveFolder, "data.logicworld"), saveBytes);
			LConsole.WriteLine("Wrote world.");
			//Save world info file:
			DataFile worldTypeFile = new DataFile(Path.Combine(saveFolder, "worldinfo.succ"));
			worldTypeFile.AutoSave = false;
			worldTypeFile.Set("WorldTypeID", extractWorldType());
			worldTypeFile.SetAtPath(0, "WorldSpawnPosition", "x");
			worldTypeFile.SetAtPath(5, "WorldSpawnPosition", "y");
			worldTypeFile.SetAtPath(0, "WorldSpawnPosition", "z");
			worldTypeFile.SaveAllData();
			LConsole.WriteLine("Wrote world info.");

			var pathExtraData = Path.Combine(saveFolder, "ExtraData");
			Directory.CreateDirectory(pathExtraData);
			LConsole.WriteLine("Writing extra data...");
			var extraData = extractExtraData();
			if(extraData == null)
			{
				return;
			}
			foreach(var (key, type, value) in extraData)
			{
				LConsole.WriteLine("Writing " + key + ": " + type);
				var keyParts = key.Split('/');
				var fixedKey = Path.Combine(keyParts);
				var dataFilePath = Path.Combine(pathExtraData, fixedKey);
				var dataFile = new DataFile(dataFilePath);
				dataFile.AutoSave = false;
				dataFile.Set("DataType", type);
				dataFile.SetNonGeneric(type, "Data", value);
				dataFile.SaveAllData();
			}

			LConsole.WriteLine("Done.");
		}

		private static List<(string, Type, object)> extractExtraData()
		{
			var extraDatas = new List<(string, Type, object)>();
			var instance = Instances.MainWorld.ExtraData;
			var fieldAllCustomData = typeof(ExtraData).GetField("AllCustomData", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fieldAllCustomData == null)
			{
				LConsole.WriteLine("ExtraData: Da list field is null :/");
				return null;
			}
			var allCustomData = fieldAllCustomData.GetValue(instance) as IDictionary;
			if(allCustomData == null)
			{
				LConsole.WriteLine("ExtraData: Da list is null or not a dictionary :/");
				return null;
			}
			var typeElement = typeof(ExtraData).Assembly.GetType("LogicWorld.SharedCode.ExtraData.ExtraDataElement");
			if(typeElement == null)
			{
				LConsole.WriteLine("ExtraData: Da type is null :/");
				return null;
			}
			var fieldType = typeElement.GetProperty("DataType", BindingFlags.Public | BindingFlags.Instance);
			var fieldValue = typeElement.GetProperty("DataValue", BindingFlags.Public | BindingFlags.Instance);
			if(fieldType == null || fieldValue == null)
			{
				LConsole.WriteLine("ExtraData: Da fields are null :/");
				return null;
			}
			foreach(DictionaryEntry entry in allCustomData)
			{
				var type = (Type) fieldType.GetValue(entry.Value);
				var value = fieldValue.GetValue(entry.Value);
				extraDatas.Add(((string) entry.Key, type, value));
			}
			return extraDatas;
		}

		private static string extractWorldType()
		{
			var types = WorldTypesManager.WorldTypesByID.Keys;
			var scene = SceneManager.GetActiveScene().name;
			if(!types.Contains(scene))
			{
				LConsole.WriteLine("The current scene '" + scene + "' is not a world-type. Set world type manually!");
				return null;
			}
			return scene;
		}

		private static bool[] extractCircuitStates()
		{
			var typeFastCircuitStateManager = typeof(RenderUpdateManager).Assembly.GetType("LogicWorld.Rendering.FastCircuitStatesManager");
			var objCircuitManager = Instances.MainWorld.CircuitStates;
			if(objCircuitManager.GetType() != typeFastCircuitStateManager)
			{
				LConsole.WriteLine("Expected circuit state manager to be of the fast type, but got: " + objCircuitManager.GetType());
				return null;
			}
			var fieldCircuitStates = typeFastCircuitStateManager.GetField("IndexedCircuitStates", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fieldCircuitStates == null)
			{
				LConsole.WriteLine("Could not find the field 'IndexedCircuitStates' in 'FastCircuitStatesManager'");
				return null;
			}
			var valueCircuitStates = fieldCircuitStates.GetValue(objCircuitManager);
			if(valueCircuitStates == null)
			{
				LConsole.WriteLine("For some reason the circuit states are 'null'...");
				return null;
			}
			if(!(valueCircuitStates is bool[] circuitStates))
			{
				LConsole.WriteLine("For some reason the circuit states are not a bool array but: " + valueCircuitStates.GetType());
				return null;
			}
			return circuitStates;
		}
	}
}
