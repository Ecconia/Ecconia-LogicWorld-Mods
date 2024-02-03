using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EccsLogicWorldAPI.Shared.AccessHelper;
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
		[Command("SaveWorld", Description = "Yeah don't, if you don't know what you are doing.")]
		public static void saveWorld()
		{
			//Ensure target folder exists:
			var savesFolder = Path.Combine(SUCC.Utilities.DefaultPath, "saves");
			//TODO: Figure out the world name - should be MP and stuff.
			var time = DateTime.Now.ToString("yyyy.MM.dd-hh·mm·ss");
			var saveFolder = Path.Combine(savesFolder, "WorldExport@" + time);
			if(Directory.Exists(saveFolder))
			{
				LConsole.WriteLine("This world folder exist already, try again: " + saveFolder);
				return;
			}
			
			//Get world bytes to export:
			bool[] circuitStates;
			try
			{
				circuitStates = extractCircuitStates();
			}
			catch(Exception e)
			{
				LConsole.WriteLine("Could not extract circuit states. Check & report stacktrace:\n" + e);
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
			List<(string, Type, object)> extraData;
			try
			{
				extraData = extractExtraData();
			}
			catch(Exception e)
			{
				LConsole.WriteLine("Could not extract extra data. Check & report stacktrace:\n" + e);
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
			var fieldAllCustomData = Fields.getPrivate(typeof(ExtraData), "AllCustomData");
			var allCustomData = Types.checkType<IDictionary>(Fields.getNonNull(fieldAllCustomData, instance));
			var typeElement = Types.findInAssembly(typeof(ExtraData), "LogicWorld.SharedCode.ExtraData.ExtraDataElement");
			var fieldType = Delegator.createPropertyGetter<object, Type>(Properties.getPublic(typeElement, "DataType"));
			var fieldValue = Delegator.createPropertyGetter<object, object>(Properties.getPublic(typeElement, "DataValue"));
			foreach(DictionaryEntry entry in allCustomData)
			{
				var type = fieldType(entry.Value);
				var value = fieldValue(entry.Value);
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
			var typeFastCircuitStateManager = Types.findInAssembly(typeof(RenderUpdateManager), "LogicWorld.Rendering.FastCircuitStatesManager");
			var objCircuitManager = Instances.MainWorld.CircuitStates;
			if(objCircuitManager.GetType() != typeFastCircuitStateManager)
			{
				throw new Exception("Expected circuit state manager to be of the fast type, but got: " + objCircuitManager.GetType());
			}
			var fieldCircuitStates = Fields.getPrivate(typeFastCircuitStateManager, "IndexedCircuitStates");
			var valueCircuitStates = Fields.getNonNull(fieldCircuitStates, objCircuitManager);
			return Types.checkType<bool[]>(valueCircuitStates);
		}
	}
}
