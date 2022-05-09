using FancyInput;
using LogicAPI.Client;
using LogicLog;

namespace CustomWirePlacer.Client
{
	public class ModClass : ClientMod
	{
		public static ILogicLogger logger;
		
		protected override void Initialize()
		{
			logger = Logger;
			
			//Hijack the original WirePlacer to do nothing and instead use the custom one.
			Hijacker.hijackWirePlacer();
			//Initialize the status overlay:
			CWP.CWPStatusDisplay.Init();
			CWP.CWPSettingsWindow.Init();
			//Initialize keys:
			CustomInput.Register<CWPContext, CWPTrigger>("CustomWirePlacer"); 
		}
	}
}
