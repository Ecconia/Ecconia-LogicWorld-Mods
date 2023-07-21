using System;
using LogicWorld.Server;

namespace EccsLogicWorldAPI.Server
{
	public static class ServiceGetter
	{
		//Just a nice wrapper over the service getter.
		public static T getService<T>()
		{
			var service = Program.Get<T>();
			if(service == null)
			{
				throw new Exception("Expected to find LogicWorld service '" + typeof(T).FullName + "', but got null");
			}
			return service;
		}
	}
}
