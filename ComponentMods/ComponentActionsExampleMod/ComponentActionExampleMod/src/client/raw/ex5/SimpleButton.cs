using System.Collections.Generic;
using JimmysUnityUtilities;
using LogicAPI.Data.BuildingRequests;
using LogicWorld.BuildingManagement;
using LogicWorld.ClientCode;
using LogicWorld.ClientCode.Decorations;
using LogicWorld.Interfaces;
using LogicWorld.References;
using LogicWorld.Rendering.Chunks;
using LogicWorld.Rendering.Components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ComponentActionExampleMod.Client.Raw.Ex5
{
	public class SimpleButton : ComponentClientCode, IPressableButton
	{
		private static readonly Color24 notPressed = new Color24(154, 72, 13);
		private static readonly Color24 pressed = new Color24(255, 100, 50);
		
		// LW does not debounce this. So it must be manually debounced.
		private bool wasPressed;
		
		protected override void Initialize()
		{
		}
		
		protected override IDecoration[] GenerateDecorations(Transform parentToCreateDecorationsUnder)
		{
			var gameObject = new GameObject("SimpleButtonPressable");
			var meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = Object.Instantiate(Meshes.BetterCube_OpenBottom);
			meshFilter.transform.localScale = new Vector3(0.2f, 0.06f, 0.2f);
			gameObject.AddComponent<MeshRenderer>().material = MaterialsCache.WorldObject(notPressed);
			var col = gameObject.AddComponent<BoxCollider>();
			col.center = new Vector3(0, 0.5f, 0);
			col.size = new Vector3(1f, 1f, 1f);
			gameObject.AddComponent<ButtonInteractable>().Button = this;
			gameObject.transform.SetParent(parentToCreateDecorationsUnder);
			
			OutlineWhenInteractableLookedAt = new MeshFilter[] {meshFilter};
			
			return new IDecoration[]
			{
				new Decoration
				{
					DecorationObject = gameObject,
					LocalPosition = new Vector3(0, 0.3f, 0),
					LocalRotation = Quaternion.identity,
					IncludeInModels = true,
					AutoSetupColliders = true,
				},
			};
		}
		
		public void MousePressDown()
		{
			if(wasPressed)
			{
				return;
			}
			wasPressed = true;
			sendPress(true);
		}
		
		public void MousePressUp()
		{
			wasPressed = false;
			sendPress(false);
		}
		
		private void sendPress(bool press)
		{
			BuildRequestManager.SendBuildRequestWithoutAddingToUndoStack(new BuildRequest_SendComponentAction(Address, new byte[] { (byte) (press ? 1 : 0) }));
		}
		
		public void pressReceived(byte value)
		{
			var color = value != 0 ? pressed : notPressed;
			Decorations[0].DecorationObject.GetComponent<MeshRenderer>().material = MaterialsCache.WorldObject(color);
		}
		
		public IReadOnlyList<MeshFilter> OutlineWhenInteractableLookedAt { get; private set; }
	}
}
