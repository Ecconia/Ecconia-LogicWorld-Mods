using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI.Data;
using LogicWorld.Building.Overhaul;
using LogicWorld.UI;

namespace EccsLogicWorldAPI.Client.Injectors.EditComponentWindow
{
	/**
	 * A helper to register custom building windows when editing a component.
	 * But with this framework no GUI framework is required, it can use any custom GUI.
	 * LogicWorlds system requires LogicWorlds GUI to be used, which has a huge dependency attachment.
	 */
	public static class ComponentEditWindowManager
	{
		private static Type typeComponentMenusManager;
		private static MethodInfo methodAddComponentsMenu;
		private static MethodInfo methodClose;
		private static Type interfaceEditComponentsMenu;
		
		private static void init()
		{
			if(interfaceEditComponentsMenu != null)
			{
				return; //Already done with setting up reflection.
			}
			typeComponentMenusManager = Types.findInAssembly(typeof(ActionWheelMenu), "LogicWorld.UI.ComponentMenusManager");
			methodAddComponentsMenu = Methods.getPrivateStatic(typeComponentMenusManager, "AddComponentMenu");
			methodClose = Methods.getPublicStatic(typeComponentMenusManager, "Close");
			interfaceEditComponentsMenu = methodAddComponentsMenu.GetParameters()[0].ParameterType;
		}
		
		/**
		 * Can be used to register a fully custom IEditComponentWindow,
		 *  which is equal to LWs interface IEditComponentMenu.
		 * It has to be registered on every world-load.
		 * For that the WorldHook API of this mod can be used.
		 *
		 * There are abstraction base classes available for modders, just as LW provides them (for itself).
		 */
		public static void RegisterWindow(IEditComponentWindow editComponentWindow)
		{
			try
			{
				init();
				var proxy = new EditComponentWindowProxy(interfaceEditComponentsMenu, editComponentWindow).GetTransparentProxy();
				methodAddComponentsMenu.Invoke(null, new object[] {proxy});
			}
			catch(Exception e)
			{
				throw new Exception("EccsLogicWorldAPI: Failed to inject a custom IEditComponentWindow/Menu", e);
			}
		}
		
		private class EditComponentWindowProxy : RealProxy
		{
			private readonly IEditComponentWindow implementation;
			
			public EditComponentWindowProxy(Type type, IEditComponentWindow implementation) : base(type)
			{
				this.implementation = implementation;
			}
			
			public override IMessage Invoke(IMessage msg)
			{
				var call = msg as IMethodCallMessage;
				if(call == null)
				{
					throw new NotSupportedException();
				}
				if("get_Priority".Equals(call.MethodName))
				{
					return new ReturnMessage(implementation.Priority, null, 0, call.LogicalCallContext, call);
				}
				if("CanEdit".Equals(call.MethodName))
				{
					var result = implementation.CanEdit((ComponentAddress) call.Args[0]);
					return new ReturnMessage(result, null, 0, call.LogicalCallContext, call);
				}
				if("CanEditCollection".Equals(call.MethodName))
				{
					var result = implementation.CanEditCollection((IEnumerable<ComponentAddress>) call.Args[0]);
					return new ReturnMessage(result, null, 0, call.LogicalCallContext, call);
				}
				if("StartEditing".Equals(call.MethodName))
				{
					implementation.StartEditing((ComponentSelection) call.Args[0]);
					return new ReturnMessage(null, null, 0, call.LogicalCallContext, call);
				}
				if("RunMenu".Equals(call.MethodName))
				{
					implementation.RunMenu();
					return new ReturnMessage(null, null, 0, call.LogicalCallContext, call);
				}
				if("DoCloseActions".Equals(call.MethodName))
				{
					implementation.DoCloseActions(out var undoRequests);
					return new ReturnMessage(null, new object[] {undoRequests}, 1, call.LogicalCallContext, call);
				}
				
				throw new Exception("EccsLogicWorldAPI: Tried to call an unknown member of the interface IEditComponentMenu: " + call.MethodName);
			}
		}
		
		public static void Close()
		{
			init();
			methodClose.Invoke(null, null);
		}
	}
}
