using ComponentActionExampleMod.Client.Raw.Ex1;
using ComponentActionExampleMod.Client.Raw.Ex5;
using ComponentActionExampleMod.Client.Raw.Ex6;
using ComponentActionExampleMod.Client.Raw.Ex7;
using LogicAPI.Client;

namespace ComponentActionExampleMod.Client
{
	public class ComponentActionExampleModClient : ClientMod
	{
		protected override void Initialize()
		{
			ColorBlockActionHandler.init();
			SimpleButtonActionHandler.init();
			EditableBlockActionHandler.init();
			DontLookAtMeActionHandler.init();
		}
	}
}
