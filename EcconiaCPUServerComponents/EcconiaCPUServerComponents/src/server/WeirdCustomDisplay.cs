using EcconiaCPUServerComponents.Shared;
using LogicWorld.Server.Circuitry;

namespace EcconiaCPUServerComponents.Server
{
	public class WeirdCustomDisplay : LogicComponent<IWeirdCustomDisplayData>
	{
		//State values:
		
		uint updateCounter; //How many more cycles should this component do logic, after a value change?
		
		// 0 - State at display
		bool data0;
		uint dataX0;
		uint dataY0;
		uint invertX0;
		uint invertY0;
		// 1 - State before the XOR gate
		bool data1;
		uint dataX1;
		uint dataY1;
		uint invertX1;
		uint invertY1;
		// 2 - State before D-Latch and invert-AND
		uint dataX2;
		uint dataY2;
		
		//Was preventing update in last tick?
		bool dirty;
		
		protected override void SetDataDefaultValues()
		{
			Data.initialize();
		}
		
		protected override void DoLogicUpdate()
		{
			//Load inputs:
			var data2 = Inputs[128].On;
			var dataX3 = inputToInt(32);
			var dataY3 = inputToInt(96);
			var invertX2 = inputToInt(0);
			var invertY2 = inputToInt(64);
			
			//Queue new updates:
			var hasInputChanged =
				invertY2 != invertY1 ||
				invertX2 != invertX1 ||
				data2 != data1 ||
				dataX3 != dataX2 ||
				dataY3 != dataY2;
			if(hasInputChanged)
			{
				updateCounter = 2;
			}
			if(updateCounter-- > 0)
			{
				QueueLogicUpdate();
			}
			
			//Figure out, if the data input data change could update the display content:
			// This is done by checking for changes and making sure that the X/Y partner has any active line.
			//TBI: Can this be optimized even more? As in capture more cases. Ignoring past instructions?
			var hasHadXInvert = invertX0 != 0 || invertX1 != 0;
			var hasHadYInvert = invertY0 != 0 || invertY1 != 0;
			var hasInvertChanged =
				invertX0 != invertX1 && hasHadYInvert ||
				invertY0 != invertY1 && hasHadXInvert;
			var hasHadXData = dataX0 != 0 || dataX1 != 0;
			var hasHadYData = dataY0 != 0 || dataY1 != 0;
			var hasDataChanged =
				dataX0 != dataX1 && hasHadYData ||
				dataY0 != dataY1 && hasHadXData ||
				data0 != data1 && hasHadXData && hasHadYData;
			
			if(hasInvertChanged || hasDataChanged)
			{
				uint dirtyLines = 0; //Bit field which has a bit set for each line that changed.
				
				//A bitfield with one bit per line. The bit indicates, that the line experienced some data change:
				var willLineBeChanged =
					(hasHadXInvert ? invertY0 | invertY1 : 0) | //For the code the past and the new inversion is relevant, include both.
					(dataX1 != 0 ? dataY1 : 0); //For the code only the next data is relevant, include that.
				
				//Map the boolean to be a whole mask of bits, so that it can be used in the bitwise logic later:
				var newValueMask = data1 ? 0xFFFFFFFF : 0;
				
				var pixelByteIndex = 0;
				uint bitMaskY = 1;
				for(var y = 0; y < 32; y++)
				{
					//Checks if there is any relevant input change for this line:
					if((willLineBeChanged & bitMaskY) == 0)
					{
						//Increase the counters, before jumping to next line:
						pixelByteIndex += 4;
						bitMaskY <<= 1;
						continue;
					}
					
					//A bunch of bit probes that have to be done for every line:
					// Each gets converted into an all 0/1 bit mask, for bitwise operations.
					var lineNowSet = (dataY1 & bitMaskY) != 0 ? 0xFFFFFFFF : 0;
					var linePreviouslyInverted = (invertY0 & bitMaskY) != 0 ? 0xFFFFFFFF : 0;
					var lineNowInverted = (invertY1 & bitMaskY) != 0 ? 0xFFFFFFFF : 0;
					
					//Read the line from the custom data to an integer:
					var tmpIndex = pixelByteIndex;
					var oldLine = (uint) (
						Data.pixelData[tmpIndex++] << 8 * 3 |
						Data.pixelData[tmpIndex++] << 8 * 2 |
						Data.pixelData[tmpIndex++] << 8 * 1 |
						Data.pixelData[tmpIndex] << 8 * 0
					);
					var newLine = oldLine;
					
					//Undo old inversion:
					newLine ^= linePreviouslyInverted & invertX0; //Pixels that had been inverted before...
					
					//Update the data:
					var pixelsThatAreNowActive = lineNowSet & dataX1; //This map indicates which of the pixels are selected by the decoding to receive new data.
					var unaffectedPixels = ~pixelsThatAreNowActive & newLine; //All old pixels gonna stay as they are.
					var affectedPixels = pixelsThatAreNowActive & newValueMask; //All new pixels gonna be set to the new data.
					newLine = unaffectedPixels | affectedPixels; //Combine new and old bits.
					
					//Apply the new inversion:
					newLine ^= lineNowInverted & invertX1; //Pixels that will be inverted...
					
					//Only mark this line as dirty and apply its value, if the line actually changed:
					if(oldLine != newLine)
					{
						dirtyLines |= bitMaskY;
						//Write the line back to custom data:
						tmpIndex = pixelByteIndex;
						Data.pixelData[tmpIndex++] = (byte) (newLine >> 8 * 3);
						Data.pixelData[tmpIndex++] = (byte) (newLine >> 8 * 2);
						Data.pixelData[tmpIndex++] = (byte) (newLine >> 8 * 1);
						Data.pixelData[tmpIndex] = (byte) (newLine >> 8 * 0);
					}
					
					pixelByteIndex += 4;
					bitMaskY <<= 1;
				}
				
				//If any of the line changed (bit is set), then cause an update on the client side:
				dirty |= dirtyLines != 0;
			}
			
			//Send data, if the display is marked as dirty, respect the update-disable peg.
			//Once the prevent-updating peg is turned off, this function will be called.
			// If there was dirty data, then it will be sent now.
			var preventUpdating = Inputs.Count == (32 * 4 + 2) ? Inputs[129].On : false;
			if(dirty && !preventUpdating)
			{
				Data.pixelData = Data.pixelData;
				dirty = false;
			}
			
			//Shift all the data down by one, for the next cycle:
			invertX0 = invertX1;
			invertX1 = invertX2;
			invertY0 = invertY1;
			invertY1 = invertY2;
			data0 = data1;
			data1 = data2;
			dataX0 = dataX1;
			dataX1 = dataX2;
			dataX2 = dataX3;
			dataY0 = dataY1;
			dataY1 = dataY2;
			dataY2 = dataY3;
		}
		
		private uint inputToInt(int start)
		{
			uint tmp = 0;
			uint bitMask = 1;
			var end = start + 31;
			for(var i = start; i <= end; i++)
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
