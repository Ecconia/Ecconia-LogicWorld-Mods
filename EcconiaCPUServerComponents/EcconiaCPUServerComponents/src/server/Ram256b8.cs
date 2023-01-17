using System;
using LogicAPI.Server.Components;
using LogicAPI.WorldDataMutations;
using LogicWorld.Server;
using LogicWorld.Server.Managers;

namespace EcconiaCPUServerComponents.Server
{
	public class Ram256b8 : LogicComponent
	{
		private static readonly IWorldUpdates worldUpdater;

		//Contains the in memory stored bytes:
		private readonly byte[] data = new byte[256];

		//Keeps track of how many of the next ticks it should run (to shift data where it belongs):
		private byte ticksToContinue;
		//Shifting data:
		//Level 3: Activation data, after the AND gate.
		private byte writeAddress3;
		private bool writeEnable3;
		private byte readAddress3;
		private bool readEnable3;
		//Level 2: Decoding data, after the Relay
		private byte writeAddress2;
		private byte readAddress2;
		//Level 1: Pre-decoder data, after the Blotter/Inverter
		private byte writeAddress1;
		private byte readAddress1;

		static Ram256b8()
		{
			//World updater only exists once while runtime anyway. So lets keep it cached statically.
			worldUpdater = Program.Get<IWorldUpdates>();
		}

		//Set this to true, so that LogicWorld knows, that it has to serialize this component before saving.
		public override bool HasPersistentValues => true;
		//No need to override the SavePersistentValuesToCustomData method, since there is no CustomData object.

		protected override void DeserializeData(byte[] customDataArray)
		{
			if(customDataArray == null)
			{
				//No need to initialize any of this mod. Is it null because its a new component?
				return;
			}
			if(customDataArray.Length == 1)
			{
				//This is a message from a dear client, probably requesting a broadcast.
				if(customDataArray[0] == 0)
				{
					//Indeed a broadcast request:
					worldUpdater.QueueMutationToBeSentToClient(new WorldMutation_UpdateComponentCustomData()
					{
						AddressOfTargetComponent = Address,
						NewCustomData = data,
					});
					return; //Done here.
				}
				else
				{
					throw new Exception("Invalid custom data message sent by client, content: " + customDataArray[0]);
				}
			}
			Array.Copy(customDataArray, data, 256);
			ticksToContinue = customDataArray[256];
			writeAddress3 = customDataArray[256 + 1];
			readAddress3 = customDataArray[256 + 2];
			writeAddress2 = customDataArray[256 + 3];
			readAddress2 = customDataArray[256 + 4];
			writeAddress1 = customDataArray[256 + 5];
			readAddress1 = customDataArray[256 + 6];
			byte booleans = customDataArray[256 + 7];
			writeEnable3 = (booleans & (1 << 0)) != 0;
			readEnable3 = (booleans & (1 << 1)) != 0;
		}

		protected override byte[] SerializeCustomData()
		{
			byte[] customDataArray = new byte[256 + 8];
			Array.Copy(data, customDataArray, 256);
			customDataArray[256] = ticksToContinue;
			customDataArray[256 + 1] = writeAddress3;
			customDataArray[256 + 2] = readAddress3;
			customDataArray[256 + 3] = writeAddress2;
			customDataArray[256 + 4] = readAddress2;
			customDataArray[256 + 5] = writeAddress1;
			customDataArray[256 + 6] = readAddress1;
			byte booleans = 0;
			if(writeEnable3)
			{
				booleans |= (1 << 0);
			}
			if(readEnable3)
			{
				booleans |= (1 << 1);
			}
			customDataArray[256 + 7] = booleans;
			return customDataArray;
		}

		/**
			I-Map:
				0-7 = LSB write MSB
				8 = Write Enable
				9-16 = LSB read MSB
				17 = Read Enable
				18-25 = LSB Write input MSB
			O-Map:
				0-7 = LSB Read output MSB
		 */
		protected override void DoLogicUpdate()
		{
			//Read all current inputs (with level - 1):
			byte writeAddress0 = inputToByte(0);
			byte readAddress0 = inputToByte(9);
			bool writeEnable2 = Inputs[8].On;
			bool readEnable2 = Inputs[17].On;
			byte writeData3 = inputToByte(18);

			//Detect changes:
			if(ticksToContinue < 3 && (writeAddress0 != writeAddress1 || readAddress0 != readAddress1))
			{
				ticksToContinue = 3;
			}
			else if(ticksToContinue < 1 && (writeEnable2 != writeEnable3 || readEnable2 != readEnable3))
			{
				ticksToContinue = 1;
			}

			//Perform actions:
			//We are using Level 3 as input, because we calculate the 4th tick.
			setOutput(readEnable3 ? data[readAddress3] : (byte) 0);
			if(writeEnable3)
			{
				//If:
				// - next cycle we will read
				// - we would not cycle next tick [Means, that the inputs have not changed]
				// - next cycle we read what we are writing now
				// - we are changing what is written to at this memory position
				//Then simulate another tick!
				if(data[writeAddress3] != writeData3)
				{
					if(readEnable2 && ticksToContinue < 1 && readAddress2 == writeAddress3)
					{
						ticksToContinue = 1; //We may have to do one more to be able to read what we just wrote.
					}
					data[writeAddress3] = writeData3;
					//Update the value change on the clients too:
					worldUpdater.QueueMutationToBeSentToClient(new WorldMutation_UpdateComponentCustomData()
					{
						AddressOfTargetComponent = Address,
						NewCustomData = new byte[] {writeAddress3, writeData3},
					});
				}
			}

			//Check if has to run more:
			if(ticksToContinue > 0)
			{
				ticksToContinue--;
				QueueLogicUpdate();
			}

			//Level 3 gets disposed, since its at Level 4 already (=> used).
			//After 3 real ticks, data here:
			writeAddress3 = writeAddress2;
			readAddress3 = readAddress2;
			writeEnable3 = writeEnable2;
			readEnable3 = readEnable2;
			//After 2 real ticks, data here:
			writeAddress2 = writeAddress1;
			readAddress2 = readAddress1;
			//After 1 real tick, data here:
			writeAddress1 = writeAddress0;
			readAddress1 = readAddress0;
		}

		private void setOutput(byte value)
		{
			for(int i = 0; i < 8; i++)
			{
				Outputs[i].On = (value & 1) != 0;
				value >>= 1;
			}
		}

		private byte inputToByte(int start)
		{
			byte tmp = 0;
			byte bitMask = 1;
			for(int i = start; i < (start + 8); i++)
			{
				if(Inputs[i].On)
				{
					tmp |= bitMask;
				}
				bitMask <<= 1;
			}
			return tmp;
		}
	}
}
