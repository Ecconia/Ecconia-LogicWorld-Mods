using CustomWirePlacer.Client.Windows;
using FancyInput;
using LogicAPI.Client;
using LogicLog;

namespace CustomWirePlacer.Client
{
	public class ModClass : ClientMod
	{
		// public static SimpleSettings settings;
		public static ILogicLogger logger;

		protected override void Initialize()
		{
			logger = Logger;

			//Hijack the original WirePlacer to do nothing and instead use the custom one.
			Hijacker.hijackWirePlacer();
			//Initialize the overlays:
			CWPStatusOverlay.Init();
			CWPHelpOverlay.Init();
			//Initialize keys:
			CustomInput.Register<CWPContext, CWPTrigger>("CustomWirePlacer");

			//Prepare settings window:
			CWPSettingsWindow.init();
		}
	}
}
