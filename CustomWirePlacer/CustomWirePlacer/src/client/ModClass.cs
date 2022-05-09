using FancyInput;
using LogicAPI.Client;

namespace CustomWirePlacer.Client
{
	public class ModClass : ClientMod
	{
		protected override void Initialize()
		{
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
