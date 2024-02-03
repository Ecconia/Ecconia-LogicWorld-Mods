using System;
using LogicAPI.Server.Components;

namespace EcconiaCPUServerComponents.Server
{
	public class RTPulser : LogicComponent
	{
		private TimeSpan pulseDuration = TimeSpan.FromSeconds(1);
		private DateTime? startOfPulse;
		private bool wasPowered;
		
		protected override void DoLogicUpdate()
		{
			Outputs[0].On = false;
			{
				bool isPowered = Inputs[0].On;
				if(isPowered && !wasPowered)
				{
					Outputs[1].On = true;
					startOfPulse = DateTime.Now;
				}
				wasPowered = isPowered;
			}
			if(startOfPulse == null)
			{
				return;
			}
			TimeSpan timeSince = DateTime.Now - startOfPulse.Value;
			if(timeSince >= pulseDuration)
			{
				startOfPulse = null;
				Outputs[0].On = true;
				Outputs[1].On = false;
			}
			QueueLogicUpdate();
		}
		
		public override bool HasPersistentValues => true;
		
		protected override void SavePersistentValuesToCustomData()
		{
			//Nothing to do, as the normal serialize will include that already.
		}
		
		protected override void DeserializeData(byte[] data)
		{
			if(data == null)
			{
				//Default initialize.
				return; //Component is already initialized.
			}
			if(data.Length == 4)
			{
				//Client update, containing the new duration:
				int timeSpanInMilliseconds = BitConverter.ToInt32(data);
				pulseDuration = TimeSpan.FromMilliseconds(timeSpanInMilliseconds);
				//TBI: Does this have to queue another LogicUpdate, or was that implied?
			}
			else if(data.Length == 9)
			{
				//Default deserialization of existing data:
				var readStream = new ReadOnlySpan<byte>(data);
				wasPowered = BitConverter.ToBoolean(readStream.Slice(0, 1)); //1
				pulseDuration = TimeSpan.FromMilliseconds(BitConverter.ToInt32(readStream.Slice(1, 4))); //4
				int millisSinceStart = BitConverter.ToInt32(readStream.Slice(5, 4)); //4
				if(millisSinceStart < 0)
				{
					startOfPulse = null;
				}
				else
				{
					startOfPulse = DateTime.Now - TimeSpan.FromMilliseconds(millisSinceStart);
				}
			}
			else
			{
				throw new Exception("Expected custom data of 9 bytes, but got: " + data.Length);
			}
		}
		
		protected override byte[] SerializeCustomData()
		{
			//Serialization of default existing data:
			Span<byte> outputBytes = stackalloc byte[9];
			BitConverter.TryWriteBytes(outputBytes, wasPowered);
			BitConverter.TryWriteBytes(outputBytes.Slice(1, 4), (int) pulseDuration.TotalMilliseconds);
			int value = startOfPulse.HasValue ? (int) (DateTime.Now - startOfPulse.Value).TotalMilliseconds : -1;
			BitConverter.TryWriteBytes(outputBytes.Slice(5, 4), value);
			return outputBytes.ToArray();
		}
	}
}
