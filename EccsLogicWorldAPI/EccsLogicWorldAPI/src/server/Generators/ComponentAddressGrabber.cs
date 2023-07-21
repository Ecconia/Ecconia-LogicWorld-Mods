using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI.Data;
using LogicAPI.Services;

namespace EccsLogicWorldAPI.Server.Generators
{
	public static class ComponentAddressGrabber
	{
		private static readonly IWorldDataMutator iWorldDataMutator;
		private static readonly Action<IWorldDataMutator, uint> idSetter;
		
		static ComponentAddressGrabber()
		{
			iWorldDataMutator = ServiceGetter.getService<IWorldDataMutator>();
			idSetter = Delegator.createPropertySetter<IWorldDataMutator, uint>(Properties.getPublic(iWorldDataMutator, "HighestComponentAddressAddedSoFar"));
		}
		
		/**
		 * Warning! This will irrevocably increase the component counter.
		 *  Do not spam this, use within reasonable amounts.
		 * LW already drains this with every new component placed.
		 *  If you are doing some component creation hackery, you can use this without concerns.
		 */
		public static ComponentAddress getNewComponentAddress()
		{
			var newID = iWorldDataMutator.HighestComponentAddressAddedSoFar + 1U;
			idSetter(iWorldDataMutator, newID);
			return new ComponentAddress(newID);
		}
	}
}
