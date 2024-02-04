using System;
using System.IO;
using EccsLogicWorldAPI.Server;
using FileDump.Shared;
using LICC;
using LogicAPI.Services;
using LogicAPI.WorldDataMutations;
using LogicWorld.Server.Circuitry;
using LogicWorld.Server.Managers;

namespace FileDump.Server
{
	public class FileDump : LogicComponent<IFileDumpData>
	{
		private static readonly IWorldUpdates iWorldUpdates;
		private static readonly IWorldDataMutator iWorldDataMutator;
		
		static FileDump()
		{
			iWorldUpdates = ServiceGetter.getService<IWorldUpdates>();
			iWorldDataMutator = ServiceGetter.getService<IWorldDataMutator>();
		}
		
		private uint lastPegCount;
		private string lastFileName;
		private ulong lastValue;
		
		private StreamWriter fileWriter;
		
		protected override void Initialize()
		{
			updateLastValues();
			open();
		}
		
		private void updateLastValues()
		{
			lastPegCount = Data.pegCount;
			lastFileName = Data.fileName;
		}
		
		protected override void DoLogicUpdate()
		{
			if(fileWriter == null)
			{
				return;
			}
			
			var value = 0uL;
			var bit = 1uL;
			for(var i = 0; i < Inputs.Count; i++)
			{
				if(Inputs[i].On)
				{
					value |= bit;
				}
				bit <<= 1;
			}
			
			if(value == lastValue)
			{
				return;
			}
			lastValue = value;
			
			for(var i = 0; i < Inputs.Count; i++)
			{
				fileWriter.Write(Inputs[i].On ? '1' : '0');
			}
			fileWriter.Write('\n');
			fileWriter.Flush();
		}
		
		public override void OnComponentDestroyed()
		{
			close();
		}
		
		public override void Dispose()
		{
			close();
			base.Dispose();
		}
		
		private void open()
		{
			var fileName = Data.fileName;
			if(fileName.Contains('/') || fileName.Contains('\\'))
			{
				LConsole.WriteLine("Illegal file name");
				return;
			}
			
			try
			{
				//This is for SinglePlayer... Thus just jump up a folder.
				var folder = "../DataDumps/";
				Directory.CreateDirectory(folder);
				fileWriter = new StreamWriter(folder + Data.fileName, true);
			}
			catch(Exception e)
			{
				//Oops.
				LConsole.WriteLine("Exception: " + e.Message);
			}
		}
		
		private void close()
		{
			if(fileWriter == null)
			{
				return;
			}
			fileWriter.Close();
			fileWriter = null;
		}
		
		protected override void OnCustomDataUpdated()
		{
			if(lastFileName == null)
			{
				//Do not process anything here, before initialize was called.
				// Once it was called, this function is called automatically.
				return;
			}
			
			//Detect the two relevant types of custom data change:
			var pegLayoutChanged = Data.pegCount != lastPegCount;
			var fileNameChanged = !Data.fileName.Equals(lastFileName);
			
			if(fileNameChanged)
			{
				LConsole.WriteLine("Name changed: '" + Data.fileName + "'");
				close();
				open();
				lastFileName = Data.fileName;
			}
			
			if(pegLayoutChanged)
			{
				LConsole.WriteLine("Name changed: '" + Data.pegCount + "'");
				//Prevent the client sending nonsense:
				if(Data.pegCount <= 32 && Data.pegCount >= 1)
				{
					var mutation = new WorldMutation_ChangeDynamicComponentPegCounts()
					{
						AddressOfTargetComponent = Address,
						NewInputCount = (int) Data.pegCount,
						NewOutputCount = 0,
					};
					iWorldUpdates.QueueMutationToBeSentToClient(mutation);
					mutation.ApplyMutation(iWorldDataMutator);
				}
				lastPegCount = Data.pegCount;
			}
		}
		
		protected override void SetDataDefaultValues()
		{
			Data.initialize();
		}
	}
}
