using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI;
using LogicWorld.Modding;

namespace EccsLogicWorldAPI.Client.AccessHelpers
{
	public static class MetaMods
	{
		private static readonly Func<MetaMod[]> getAllModMetas;
		
		public static MetaMod[] getAllMetaMods => getAllModMetas();
		
		static MetaMods()
		{
			getAllModMetas = Delegator.createStaticFieldGetter<MetaMod[]>(
				Fields.getPrivateStatic(
					Types.findInAssembly(typeof(Mods), "LogicWorld.Modding.Loading.ModLoader"),
					"MetaMods"
				)
			);
		}
	}
}
