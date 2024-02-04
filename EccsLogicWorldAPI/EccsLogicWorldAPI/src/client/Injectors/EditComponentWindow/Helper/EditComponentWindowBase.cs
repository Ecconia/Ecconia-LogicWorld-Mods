using System.Collections.Generic;
using System.Linq;
using LogicAPI.Data;
using LogicAPI.Data.BuildingRequests;
using LogicUI;
using LogicWorld.Building.Overhaul;
using LogicWorld.Interfaces;

namespace EccsLogicWorldAPI.Client.Injectors.EditComponentWindow.Helper
{
	public abstract class EditComponentWindowBase<T> : IEditComponentWindow
		where T : ToEditComponentInfo, new()
	{
		private bool EnableUIBackShortcut;
		
		protected IReadOnlyList<T> ComponentsBeingEdited { get; private set; }
		
		protected T FirstComponentBeingEdited { get; private set; }
		
		protected virtual bool EditingChangesComponentData => false;
		
		protected EditComponentWindowBase()
		{
			//This does not get loaded by Unity, so the constructor registering this is fine.
			ComponentEditWindowManager.RegisterWindow(this);
		}
		
		//### Internal helpers:
		
		protected virtual T CreateComponentInfoFor(ComponentAddress cAddress)
		{
			var componentInfo = new T();
			componentInfo.Address = cAddress;
			var mainWorld = Instances.MainWorld;
			componentInfo.Component = mainWorld.Data.Lookup(cAddress);
			componentInfo.ClientCode = mainWorld.Renderer.Entities.GetClientCode(cAddress);
			if(EditingChangesComponentData)
			{
				componentInfo.setBeforeEditingCustomData();
			}
			return componentInfo;
		}
		
		private IEnumerable<BuildRequest> GetUndoRequestsOnClose()
		{
			foreach(var editingInfo in ComponentsBeingEdited)
			{
				if(editingInfo.hasCustomDataChanged())
				{
					yield return new BuildRequest_UpdateComponentCustomData(editingInfo.Address, editingInfo.BeforeEditingCustomData);
				}
			}
		}
		
		//### Interface implementation:
		
		public virtual int Priority => 0;
		
		bool IEditComponentWindow.CanEdit(ComponentAddress cAddress) => CanEdit(CreateComponentInfoFor(cAddress));
		
		protected abstract bool CanEdit(T componentInfo);
		
		bool IEditComponentWindow.CanEditCollection(IEnumerable<ComponentAddress> collection)
		{
			var componentsToEdit = new List<T>();
			foreach(var cAddress in collection)
			{
				//TBI: Is this cast free? Does C# have another way for this implicit casting.
				if(!(this as IEditComponentWindow).CanEdit(cAddress))
				{
					return false;
				}
				componentsToEdit.Add(CreateComponentInfoFor(cAddress));
			}
			return CanEditCollection(componentsToEdit);
		}
		
		protected virtual bool CanEditCollection(IReadOnlyList<T> selection) => true;
		
		public void StartEditing(ComponentSelection selection)
		{
			var editingInfoList = new List<T>();
			foreach(var cAddress in selection)
			{
				editingInfoList.Add(CreateComponentInfoFor(cAddress));
			}
			ComponentsBeingEdited = editingInfoList;
			//Potentially redundant wrapping, but negligible:
			FirstComponentBeingEdited = CreateComponentInfoFor(selection.FirstComponentInSelection);
			EnableUIBackShortcut = true;
			OnStart();
		}
		
		public void RunMenu()
		{
			//In theory there could be other shortcuts being handled here too,
			// but there is none that makes sense for editing windows.
			if(EnableUIBackShortcut && UITrigger.Back.DownThisFrame())
			{
				CloseWindow();
				return;
			}
			OnRun();
		}
		
		public void DoCloseActions(out BuildRequest[] undoRequests)
		{
			OnClose();
			undoRequests = GetUndoRequestsOnClose().ToArray();
			ComponentsBeingEdited = null;
			FirstComponentBeingEdited = default;
			EnableUIBackShortcut = true;
		}
		
		//### Custom stuff:
		
		/**
		 * Set if the editing should stop, once the player hits "back".
		 *  Can be temporary disabled, for e.g keyboard edits.
		 */
		public void SetUIBackShortcutEnabled(bool newState)
		{
			EnableUIBackShortcut = newState;
		}
		
		/**
		 * Can be called to stop editing.
		 */
		protected void CloseWindow()
		{
			ComponentEditWindowManager.Close();
		}
		
		/**
		 * Called when editing is starting.
		 */
		protected virtual void OnStart()
		{
		}
		
		/**
		 * Called every frame while editing.
		 */
		protected virtual void OnRun()
		{
		}
		
		/**
		 * Called when editing is done.
		 */
		protected virtual void OnClose()
		{
		}
	}
}
