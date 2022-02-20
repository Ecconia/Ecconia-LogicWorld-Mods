//Needed for 'LogicComponent':
using LogicWorld.Server.Circuitry;

//Needed for my displays data:
using Ecconia.CPUServerComponents.Shared;

namespace Ecconia.CPUServerComponents.Server
{
	public class WeirdCustomDisplay : LogicComponent<IFirstDisplayData>
	{
		//State values:

		int updateCounter;

		// 0 - State at display
		bool data0;
		int dataX0;
		int dataY0;
		int invertX0;
		int invertY0;
		// 1 - State before the XOR gate
		bool data1;
		int dataX1;
		int dataY1;
		int invertX1;
		int invertY1;
		// 2 - State before D-Latch and invert-AND
		bool data2;
		int dataX2;
		int dataY2;
		int invertX2;
		int invertY2;
		// 3 - State before data-AND
		int dataX3;
		int dataY3;

		protected override void SetDataDefaultValues()
		{
			base.Data.pixelData = new byte[128];
		}

		protected override void DoLogicUpdate()
		{
			// LConsole.WriteLine("--------------------------\n" +
			//                    "Update counter was: {0}", updateCounter);

			//Load inputs:
			bool data3 = base.Inputs[128].On;
			int dataX4 = inputToInt(32);
			int dataY4 = inputToInt(96);
			int invertX3 = inputToInt(0);
			int invertY3 = inputToInt(64);

			//Queue new updates:
			bool hasInputChanged =
				invertY3 != invertY2 ||
				invertX3 != invertX2 ||
				data3 != data2 ||
				dataX4 != dataX3 ||
				dataY4 != dataY3;
			if(hasInputChanged)
			{
				updateCounter = 3;
				// LConsole.WriteLine("Update counter set: {0}", updateCounter);
			}
			if(updateCounter-- > 0)
			{
				base.QueueLogicUpdate();
				// LConsole.WriteLine("Queued moar update.");
			}

			// LConsole.WriteLine("Data values: 3: {0} 2: {1} 1: {2} 0: {3}", data3, data2, data1, data0);
			// LConsole.WriteLine("DataX values: 4: {0} 3: {1} 2: {2} 1: {3} 0: {4}", dataX4, dataX3, dataX2, dataX1, dataX0);
			// LConsole.WriteLine("DataY values: 4: {0} 3: {1} 2: {2} 1: {3} 0: {4}", dataY4, dataY3, dataY2, dataY1, dataY0);
			// LConsole.WriteLine("InvertX values: 3: {0} 2: {1} 1: {2} 0: {3}", invertX3, invertX2, invertX1, invertX0);
			// LConsole.WriteLine("InvertY values: 3: {0} 2: {1} 1: {2} 0: {3}", invertY3, invertY2, invertY1, invertY0);

			//Handle the data:
			bool hasInvertChanged = invertX0 != invertX1 || invertY0 != invertY1;
			bool hasDataChanged = dataX0 != dataX1 || dataY0 != dataY1 || data0 != data1; //TODO: Optimize condition? Or trust player?

			bool dirty = false;
			if(hasInvertChanged || hasDataChanged)
			{
				// LConsole.WriteLine("Performing display update!");
				int pixelByteIndex = 0;
				byte bitMaskByte = 1;
				int bitMaskY = 1;
				for(int y = 0; y < 32; y++)
				{
					// bool linePreviouslySet = (dataY0 & bitMaskY) != 0;
					bool lineNowSet = (dataY1 & bitMaskY) != 0;
					bool linePreviouslyInverted = (invertY0 & bitMaskY) != 0;
					bool lineNowInverted = (invertY1 & bitMaskY) != 0;

					int bitMaskX = 1;
					for(int x = 0; x < 32; x++)
					{
						//Process pixel:
						bool oldBit = (base.Data.pixelData[pixelByteIndex] & bitMaskByte) != 0;

						// bool columnPreviouslySet = (dataX0 & bitMaskX) != 0;
						bool columnNowSet = (dataX1 & bitMaskX) != 0;
						bool columnPreviouslyInverted = (invertX0 & bitMaskX) != 0;
						bool columnNowInverted = (invertX1 & bitMaskX) != 0;

						// bool previouslySet = columnPreviouslySet && linePreviouslySet && data0;
						bool nowSet = columnNowSet && lineNowSet;
						bool previouslyInverted = columnPreviouslyInverted && linePreviouslyInverted;
						bool nowInverted = columnNowInverted && lineNowInverted;

						if(nowSet)
						{
							bool intendedPixelState = previouslyInverted ? !data1 : data1;
							if(intendedPixelState != oldBit)
							{
								if(intendedPixelState)
								{
									base.Data.pixelData[pixelByteIndex] |= bitMaskByte;
								}
								else
								{
									base.Data.pixelData[pixelByteIndex] &= (byte) ~bitMaskByte;
								}
								dirty = true;
								oldBit = intendedPixelState;
							}
						}

						if(nowInverted != previouslyInverted)
						{
							if(oldBit)
							{
								base.Data.pixelData[pixelByteIndex] &= (byte) ~bitMaskByte;
							}
							else
							{
								base.Data.pixelData[pixelByteIndex] |= bitMaskByte;
							}
							dirty = true; //Even if this action would invert the previous action, the display is meant to have these run separate from each other, so this is fine.
						}

						//Prepare for the next pixel:
						bitMaskByte <<= 1;
						if(bitMaskByte == 0)
						{
							bitMaskByte = 1;
							pixelByteIndex++;
						}
						bitMaskX <<= 1;
					}
					bitMaskY <<= 1;
				}
			}

			if(dirty)
			{
				base.Data.pixelData = base.Data.pixelData;
			}

			//Shift all the data down by one, for the next cycle.
			invertX0 = invertX1;
			invertX1 = invertX2;
			invertX2 = invertX3;
			invertY0 = invertY1;
			invertY1 = invertY2;
			invertY2 = invertY3;
			data0 = data1;
			data1 = data2;
			data2 = data3;
			dataX0 = dataX1;
			dataX1 = dataX2;
			dataX2 = dataX3;
			dataX3 = dataX4;
			dataY0 = dataY1;
			dataY1 = dataY2;
			dataY2 = dataY3;
			dataY3 = dataY4;
		}

		private int inputToInt(int start)
		{
			int tmp = 0;
			int bitMask = 1;
			for(int i = start; i <= (start + 31); i++)
			{
				if(base.Inputs[i].On)
				{
					tmp |= bitMask;
				}
				bitMask <<= 1;
			}
			return tmp;
		}

		private void setPixel(int x, int y, bool state)
		{
			int index = x + y * 32;

			int byteIndex = index / 8;
			byte bitMask = (byte) (1 << (index % 8));

			//LConsole.WriteLine("Set byte: {0} and bit {1}", byteIndex, (index % 8));
			byte oldByte = base.Data.pixelData[byteIndex];
			bool isSet = (oldByte & bitMask) != 0;
			if(state != isSet)
			{
				if(state)
				{
					//Set:
					base.Data.pixelData[byteIndex] = (byte) (oldByte | bitMask);
				}
				else
				{
					//Unset:
					base.Data.pixelData[byteIndex] = (byte) (oldByte & (~bitMask));
				}
				//"Hack" to trigger the change listener:
				base.Data.pixelData = base.Data.pixelData;
			}
		}
	}
}
