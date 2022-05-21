using LogicUI.MenuTypes.ConfigurableMenus;

namespace EccsWindowHelper.Client
{
	//This class is in charge of holding a few relevant parts of the window.
	// However the only thing currently relevant are the window settings.
	public class WindowController
	{
		public readonly ConfigurableMenuSettings settingsController;

		public WindowController(ConfigurableMenuSettings settingsController)
		{
			this.settingsController = settingsController;
		}
	}
}
