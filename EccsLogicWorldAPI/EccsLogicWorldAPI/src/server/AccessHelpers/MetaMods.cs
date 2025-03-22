using System;
using System.Collections.Generic;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI;
using LogicWorld.Server.Managers;

namespace EccsLogicWorldAPI.Server.AccessHelpers
{
	public static class MetaMods
	{
		private static readonly IModManager modManager;
		private static readonly Func<IModManager, IList<MetaMod>> getAllModMetas;
		
		public static IList<MetaMod> getAllMetaMods => getAllModMetas(modManager);
		
		static MetaMods()
		{
			modManager = ServiceGetter.getService<IModManager>();
			getAllModMetas = Delegator.createFieldGetter<IModManager, IList<MetaMod>>(
				Fields.getPrivate(typeof(ModManager), "MetaMods")
			);
		}
	}
}
