using FileDump.Shared;
using LogicWorld.Rendering.Components;

namespace FileDump.Client
{
	public class FileDump : ComponentClientCode<IFileDumpData>
	{
		//Called when the component gets created (which also means after its pegs have changed):
		protected override void Initialize()
		{
		}
		
		//Called when custom data changes (server or client dictated!):
		protected override void DataUpdate()
		{
			// //If the component is being edited right now, but something/one edits the data. Re-Initialize the edit window, so that the outline updates:
			// EditWindowRefresher.updateEditWindow(Address);
		}
		
		protected override void SetDataDefaultValues()
		{
			Data.initialize();
		}
	}
}
