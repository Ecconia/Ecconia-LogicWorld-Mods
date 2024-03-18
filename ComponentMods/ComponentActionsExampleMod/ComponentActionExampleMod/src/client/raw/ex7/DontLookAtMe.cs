using System.Diagnostics;
using JimmysUnityUtilities;
using LogicAPI.Data.BuildingRequests;
using LogicWorld.BuildingManagement;
using LogicWorld.Interfaces;
using LogicWorld.Players;
using LogicWorld.Rendering.Chunks;
using LogicWorld.Rendering.Components;
using UnityEngine;

namespace ComponentActionExampleMod.Client.Raw.Ex7
{
	public class DontLookAtMe : ComponentClientCode
	{
		private VisibilityDetector visibilityDetector;
		
		private const long millisBetween = 20;
		private long lastTrigger;
		
		protected override void InitializeInWorld()
		{
			visibilityDetector.OnBecomeVisible += () => {
				lastTrigger = 0;
				QueueFrameUpdate();
			};
		}
		
		protected override void FrameUpdate()
		{
			if(visibilityDetector == null || !PlacedInMainWorld)
			{
				// Only trigger when in main world. During item render this would fail.
				return; // Not yet setup. Thanks LW for this callback.
			}
			
			if(
				Stopwatch.GetTimestamp() - lastTrigger > millisBetween
				&& Instances.MainWorld.Renderer.Entities.GetBlockEntitiesAt(Address)[0].Collider.Raycast(PlayerControllerManager.CameraRay(), out _, 50f)
			) {
				//Hit!
				lastTrigger = Stopwatch.GetTimestamp(); //Whatever unit that is. Idc about correctness in this example.
				BuildRequestManager.SendBuildRequestWithoutAddingToUndoStack(new BuildRequest_SendComponentAction(Address, null));
			}
			if(visibilityDetector.IsVisible)
			{
				ContinueUpdatingForAnotherFrame();
			}
		}
		
		protected override IDecoration[] GenerateDecorations(Transform parentToCreateDecorationsUnder)
		{
			var go = new GameObject();
			go.AddComponent<MeshRenderer>();
			go.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			visibilityDetector = go.AddComponent<VisibilityDetector>();
			go.transform.SetParent(parentToCreateDecorationsUnder);
			
			return new IDecoration[]
			{
				new Decoration()
				{
					DecorationObject = go,
					LocalRotation = Quaternion.identity,
					LocalPosition = new Vector3(0, 0.15f, 0),
					IncludeInModels = false,
					AutoSetupColliders = false,
				},
			};
		}
	}
}
