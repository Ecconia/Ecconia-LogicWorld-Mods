using LogicAPI.Data;
using LogicAPI.Interfaces;
using LogicWorld.Audio;
using LogicWorld.SharedCode.Components;

namespace ComponentActionExampleMod.Client.Raw.Ex7
{
	public class DontLookAtMeActionHandler : IComponentActionMutationHandler
	{
		public static void init()
		{
			ComponentActionMutationManager.RegisterHandler(new DontLookAtMeActionHandler(), "ComponentActionExampleMod.DontLookAtMe");
		}
		
		public void HandleComponentAction(ComponentAddress componentAddress, IComponentInWorld componentInWorld, byte[] actionData)
		{
			if(actionData == null)
			{
				SoundPlayer.PlaySoundAt("MHG.PlaceOnBoard", componentAddress);
			}
		}
	}
}
