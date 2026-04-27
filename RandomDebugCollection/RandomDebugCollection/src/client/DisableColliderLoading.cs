using System;
using EccsLogicWorldAPI.Shared.AccessHelper;
using HarmonyLib;
using LogicAPI.Data;
using LogicWorld.Physics;
using LogicWorld.References;
using LogicWorld.Rendering;
using LogicWorld.Rendering.Chunks;
using LogicWorld.Rendering.Data;
using LogicWorld.SharedCode;
using LogicWorld.SharedCode.Components;
using UnityEngine;
using Types = EccsLogicWorldAPI.Shared.AccessHelper.Types;

namespace RandomDebugCollection.Client
{
	public static class DisableColliderLoading
	{
		private static BoxCollider collider;
		private static Func<MaterialType, Mesh, GpuColor, Transform, ColliderData, Transform, ColliderReferenceData, Vector3, Quaternion, Vector3, bool, RenderedEntity> create;
		
		public static void disableColliderLoading()
		{
			var cClass = Types.findInAssembly(typeof(RenderUpdateManager), "LogicWorld.Rendering.Colliders.EntityCollidersManager");
			var mCreate = Methods.getPublicStatic(cClass, "GetEntityCollider");
			var patch = Methods.getPrivateStatic(typeof(DisableColliderLoading), nameof(returnNull));
			new Harmony("RandomDebugCollection").Patch(mCreate, new HarmonyMethod(patch));
			
			var cClass2 = Types.findInAssembly(typeof(RenderUpdateManager), "LogicWorld.Rendering.EntityGenerator");
			var mCreate2 = Methods.getPublicStatic(cClass2, "GetWireEntity");
			var patch2 = Methods.getPrivateStatic(typeof(DisableColliderLoading), nameof(patchGetWireEntity));
			new Harmony("RandomDebugCollection").Patch(mCreate2, new HarmonyMethod(patch2));
			
			var method = Methods.getPrivateStatic(typeof(RenderedEntity), "Create");
			create = Delegator.createStaticMethodCall<RenderedEntity, MaterialType, Mesh, GpuColor, Transform, ColliderData, Transform, ColliderReferenceData, Vector3, Quaternion, Vector3, bool>(method);
			collider = new BoxCollider();
		}
		
		private static bool returnNull(ref Collider __result)
		{
			__result = collider;
			return false;
		}
		
		private static bool patchGetWireEntity(ref RenderedEntity __result, WireRenderData renderData, Transform colliderParent, Transform outlinesParent, WireAddress wAddress)
		{
			__result = create(
				MaterialType.SolidColor,
				Meshes.DoubleOpenEndedCube,
				Colors.CircuitOff,
				
				colliderParent,
				ColliderData.Wire,
				outlinesParent,
				ColliderReferenceData.ForWire(wAddress),
				
				renderData.WorldPosition,
				renderData.WorldRotation,
				renderData.Scale,
				
				true
			);
			return false;
		}
	}
}
