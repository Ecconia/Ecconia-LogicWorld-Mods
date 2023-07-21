using System;
using System.Collections.Generic;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LogicAPI.Data;
using LogicWorld.Interfaces;
using LogicWorld.Rendering.Components;
using UnityEngine;

namespace EccsLogicWorldAPI.Client.AccessHelpers
{
	/*
	 * Functions to provide the missing utility to change pegs height.
	 *  Yes one can use these to make a peg any cubic shape.
	 *
	 * Just keep in mind, the shape of an output peg always implies, that it takes 1 tick.
	 * Do never make an input peg look like an output peg. That will only confuse players. And cause havoc.
	 */
	public static class PegScale
	{
		private static readonly Func<ComponentClientCode, IReadOnlyList<IRenderedEntity>> inputGetter;
		private static readonly Func<ComponentClientCode, IReadOnlyList<IRenderedEntity>> outputGetter;
		
		static PegScale()
		{
			try
			{
				inputGetter = Delegator.createFieldGetter<ComponentClientCode, IReadOnlyList<IRenderedEntity>>(Fields.getPrivate(typeof(ComponentClientCode), "InputEntities"));
				outputGetter = Delegator.createFieldGetter<ComponentClientCode, IReadOnlyList<IRenderedEntity>>(Fields.getPrivate(typeof(ComponentClientCode), "OutputEntities"));
			}
			catch(Exception e)
			{
				throw new Exception("[EccLwApi]Failure, while trying to create getters for inputs/outputs of a component.", e);
			}
		}
		
		public static void SetInputScale(IWorldRenderer WorldRenderer, ComponentClientCode component, int index, Vector3 scale)
		{
			inputGetter(component)[index].Scale = scale * 0.3f;
			WorldRenderer.EntityManager.UpdateWorldWirePoint(new InputAddress(component.Address, index));
		}
		
		public static void SetInputHeight(IWorldRenderer WorldRenderer, ComponentClientCode component, int index, float scale)
		{
			var peg = inputGetter(component)[index];
			var oldScale = peg.Scale;
			oldScale[1] = scale * 0.3f;
			peg.Scale = oldScale;
			WorldRenderer.EntityManager.UpdateWorldWirePoint(new InputAddress(component.Address, index));
		}
		
		public static void SetOutputScale(IWorldRenderer WorldRenderer, ComponentClientCode component, int index, Vector3 scale)
		{
			outputGetter(component)[index].Scale = scale * 0.3f;
			WorldRenderer.EntityManager.UpdateWorldWirePoint(new OutputAddress(component.Address, index));
		}
	}
}
