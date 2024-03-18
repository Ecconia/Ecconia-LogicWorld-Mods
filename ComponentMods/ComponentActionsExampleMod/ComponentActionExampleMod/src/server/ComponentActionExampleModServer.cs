using ComponentActionExampleMod.Server.Raw.Ex2;
using ComponentActionExampleMod.Server.Raw.Ex3;
using ComponentActionExampleMod.Server.Raw.Ex5;
using ComponentActionExampleMod.Server.Raw.Ex6;
using ComponentActionExampleMod.Server.Raw.Ex7;
using LogicAPI.Server;

namespace ComponentActionExampleMod.Server
{
	public class ComponentActionExampleModServer : ServerMod
	{
		protected override void Initialize()
		{
			AddSubberActionHandler.init();
			PulseByEditActionHandler.init();
			SimpleButtonActionHandler.init();
			SimpleButtonWorldActionHandler.init();
			EditableBlockActionHandler.init();
			DontLookAtMeActionHandler.init();
		}
	}
}
