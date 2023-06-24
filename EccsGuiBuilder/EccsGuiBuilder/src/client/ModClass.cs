using EccsGuiBuilder.Client.Components;
using EccsLogicWorldAPI.Client.Hooks;
using LogicAPI.Client;

namespace EccsGuiBuilder.Client
{
	public class ModClass : ClientMod
	{
		protected override void Initialize()
		{
			WorldHook.worldLoading += () =>
			{
				VanillaStore.init();
				CustomStore.init();
			};
		}
	}
}
