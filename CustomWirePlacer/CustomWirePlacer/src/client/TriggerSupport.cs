using System;
using FancyInput;

namespace CustomWirePlacer.Client
{
	public static class TriggerSupport
	{
		public static bool DownThisFrame(this CWPTrigger trigger) => CustomInput.DownThisFrame((InputTrigger) (Enum) trigger);
		
		public static bool UpThisFrame(this CWPTrigger trigger) => CustomInput.UpThisFrame((InputTrigger) (Enum) trigger);
		
		public static bool Held(this CWPTrigger trigger) => CustomInput.Held((InputTrigger) (Enum) trigger);
	}
}
