using System.Collections.Generic;
using EcconiaCPUServerComponents.Shared;
using FancyInput;
using JimmysUnityUtilities;
using LICC;
using LogicUI.MenuTypes;
using LogicAPI.Data;
using LogicWorld.Audio;
using LogicWorld.ClientCode;
using LogicWorld.ClientCode.Decorations;
using LogicWorld.ClientCode.Resizing;
using LogicWorld.GameStates;
using LogicWorld.Interfaces;
using LogicWorld.Physics;
using LogicWorld.Players;
using LogicWorld.References;
using LogicWorld.Rendering.Chunks;
using LogicWorld.Rendering.Components;
using TMPro;
using UnityEngine;

namespace EcconiaCPUServerComponents.Client
{
	public static class Extensions
	{
		public static Color24 lighten(this Color24 color)
		{
			const float factor = 0.4f;
			return new Color24(
				(byte) (color.r + (255 - color.r) * factor),
				(byte) (color.g + (255 - color.g) * factor),
				(byte) (color.b + (255 - color.b) * factor)
			);
		}
	}

	public class FlatKey : ComponentClientCode<IFlatKeyData>,
	                       IResizableX,
	                       IResizableZ,
	                       IPressableButton,
	                       IColorableClientCode
	{
		//Command:

		[Command("EditKeyLabel")]
		public static void editKeyLabel(string text)
		{
			//Yeah no clue what these layers are, but I take them all.
			HitInfo info = PlayerCaster.CameraCast(Masks.Default | Masks.Environment | Masks.Structure | Masks.Peg | Masks.PlayerModel);
			ComponentAddress componentAddress = info.cAddress;
			if(componentAddress == null)
			{
				LConsole.WriteLine("Look at a CustomPanelKey to edit it.");
				return;
			}

			IComponentClientCode clientCode = Instances.MainWorld.Renderer.Entities.GetClientCode(componentAddress);
			if(clientCode == null || clientCode.GetType() != typeof(FlatKey))
			{
				IComponentInWorld componentInWorld = Instances.MainWorld.Data.Lookup(componentAddress);
				string type = Instances.MainWorld.ComponentTypes.GetTextID(componentInWorld.Data.Type);
				LConsole.WriteLine("Look at a CustomPanelKey to edit it. Not at: " + type);
				return;
			}

			FlatKey key = (FlatKey) clientCode;
			key.Data.label = text.Replace("\\n", "\n");

			LConsole.WriteLine("Set label text!");
		}

		//Constants:
		private static readonly SoundEffect effect = SoundEffectDatabase.GetSoundEffectByTextID("EcconiaCPUServerComponents.FlatKeySound");

		private VisibilityDetector visibilityDetector;

		//Remember last size, to detect changes. Memory overhead in favor of runtime overhead.
		private int previousSizeX;
		private int previousSizeZ;

		/*
		 * Keeps an internal state, if the button is currently being pressed.
		 * TBI: I do not like this solution, since it adds yet another field to the component.
		 * But first I have to understand the general structure of this framework.
		 *
		 * Runs a frame update after state change.
		 */
		private bool manuallyPressed { get; set; }
		/*
		 * Stores if in the last frame, the client was pressing this key actively.
		 */
		private bool clientWasPressingKey;
		/*
		 * Only used to check if the local state has changed, whatever it was.
		 */
		private bool keyWasPressed;

		protected override void DataUpdate()
		{
			GameObject keycapGameObject = GetDecorations()[0].DecorationObject;
			GameObject labelGameObject = GetDecorations()[1].DecorationObject;

			TextMeshPro text = labelGameObject.GetComponent<TextMeshPro>();
			if(SizeX != previousSizeX || SizeZ != previousSizeZ)
			{
				//Update solid:
				SetBlockScale(0, new Vector3(SizeX, 1f / 3f, SizeZ));
				SetBlockPosition(0, new Vector3(SizeX / 2f - 0.5f, 0, SizeZ / 2f - 0.5f));
				SetBlockScale(1, new Vector3(2f / 3f + SizeX - 1, 5f / 6f, 2f / 3f + SizeZ - 1));
				SetBlockPosition(1, new Vector3(SizeX / 2f - 0.5f, 0, SizeZ / 2f - 0.5f));
				//Update output:
				SetOutputPosition(0, new Vector3((SizeX - 1) * 0.5f, -5f / 6f, (SizeZ - 1) * 0.5f));
				//Update keycap:
				keycapGameObject.GetComponent<MeshFilter>().mesh = KeycapLab.getKeycapMeshFor(SizeX, SizeZ);
				BoxCollider collider = keycapGameObject.GetComponent<BoxCollider>();
				collider.center = new Vector3((SizeX - 1) * 0.15f, -0.3f * 0.1f, (SizeZ - 1) * 0.15f);
				collider.size = new Vector3(SizeX * 0.3f - 0.015f, 0.3f * 0.2f, SizeZ * 0.3f - 0.015f); //Shrink a bit (same amount as the visuals in the KeyCapLab). To not let the bounding boxes collide - breaks stuff.
				//Update text bounds:
				text.rectTransform.sizeDelta = new Vector2(Data.sizeX * 0.3f - 0.02f, Data.sizeZ * 0.3f - 0.02f);
				Vector3 worldPos = Component.WorldPosition
					+ this.Component.WorldRotation * new Vector3(
						(SizeX - 1) * 0.15f,
						0.1f + 0.3f * 0.2f + 0.001f,
						(SizeZ - 1) * 0.15f
					) - labelGameObject.transform.parent.parent.localPosition;
				labelGameObject.transform.localPosition = worldPos;
				//Update values:
				previousSizeX = SizeX;
				previousSizeZ = SizeZ;
			}
			//Always update:
			{
				//Press state updates:
				keyStateUpdate();
				//Data update:
				text.color = Data.KeyLabelColor.WithOpacity();
				text.text = Data.label.IsNullOrEmpty() ? ((RawInput) this.Data.BoundInput).DisplayName() : Data.label.Replace(" ", "<color=#0000>.</color>");
			}
		}

		protected override void InitializeInWorld()
		{
			//Became visible to the camera, start looping functionality.
			visibilityDetector.OnBecomeVisible += this.QueueFrameUpdate;
		}

		protected override void FrameUpdate()
		{
			//Required, else NPE on SoundPlayer.
			if(!PlacedInMainWorld)
			{
				return;
			}

			//Is this client currently pressing this key?
			bool clientIsPressingKey = isButtonCurrentlyPressable() && (manuallyPressed || isPressingWithKeyboard());
			if(clientIsPressingKey != clientWasPressingKey)
			{
				//TODO: Only unpress, when no other is pressing this.
				//Update game state (globally) with what we did with the key:
				Data.KeyDown = clientIsPressingKey; //This might turn off the key for someone else!
				//Update the old pressing state:
				clientWasPressingKey = clientIsPressingKey;

				//Update the client state:
				keyStateUpdate();
			}

			//Keep updating as long as it is visible.
			if(visibilityDetector.IsVisible)
			{
				base.ContinueUpdatingForAnotherFrame();
			}
		}

		private bool isPressingWithKeyboard()
		{
			//This method is heavily "inspired" by the original LogicWorld-Key source code.

			return ((RawInput) Data.BoundInput).Held() //First of all, the key has to be down.
				&& GameStateManager.CurrentStateID == "MHG.InChair" //We checked this before, but keys only works when in a chair - cause I decided so (=> many advantages).
				&& rangeCheck() //Check if the distance is not higher than max allowed interaction.
				&& isRoughlySeeingEachOther(); //Key needs to be "in front of" player and player "in front of" key.

			bool rangeCheck()
			{
				return Vector3.Distance(this.Component.WorldPosition, PlayerControllerManager.PlayerCamera.CameraWorldspacePosition) < PlayerControllerManager.ReachDistance;
			}

			bool isRoughlySeeingEachOther()
			{
				Vector3 keyPosition = Component.WorldPosition;
				Quaternion keyAlignment = Component.WorldRotation;
				Ray ray = PlayerControllerManager.PlayerCamera.GetCameraRay();
				Vector3 cameraSpacePos = Quaternion.FromToRotation(ray.direction, Vector3.forward) * (keyPosition - ray.origin);
				Vector3 keySpacePos = keyAlignment.Inverse() * (ray.origin - keyPosition);

				return cameraSpacePos.z > 0 && keySpacePos.y > 0;
			}
		}

		private void keyStateUpdate()
		{
			//Always do visual update:
			GetDecorations()[0].DecorationObject.GetComponent<MeshRenderer>().material = MaterialsCache.WorldObject(Data.KeyDown ? Data.KeyColor.lighten() : Data.KeyColor);

			//TODO: If key is globally not pressed, but this client still presses it, force press the button again.
			if(!PlacedInMainWorld || keyWasPressed == Data.KeyDown)
			{
				//Component is not ready to be displayed yet, will crash SoundPlayer!
				//The last displayed state is the same as the global state.
				return;
			}
			//Do audible update:
			if(Data.KeyDown)
			{
				//The key was switched on. Play sound:
				SoundPlayer.PlaySoundAt(effect, Address);
			}

			//Update the displayed state.
			keyWasPressed = Data.KeyDown;
		}

		private bool isButtonCurrentlyPressable()
		{
			return
				visibilityDetector.IsVisible //The KeyCap needs to be roughly visible.
				//Questionable solution: Should instead ask the current state for properties, cause this is not extendable.
				&& (GameStateManager.CurrentStateID == "MHG.Building" || GameStateManager.CurrentStateID == "MHG.InChair") //Only let the keys be pressable, when building (free-cam) or in chair.
				&& !ToggleableSingletonMenu<FancyPantsConsole.Console>.MenuIsVisible; //This window shadows over each game state, so manual query is required.
		}

		protected override IList<IDecoration> GenerateDecorations()
		{
			//Keycap:
			GameObject keycapGameObject = new GameObject();
			MeshFilter meshFilter = keycapGameObject.AddComponent<MeshFilter>();
			keycapGameObject.AddComponent<MeshRenderer>();
			keycapGameObject.AddComponent<BoxCollider>();
			keycapGameObject.AddComponent<ButtonInteractable>().Button = this;
			visibilityDetector = keycapGameObject.AddComponent<VisibilityDetector>();
			OutlineWhenInteractableLookedAt = new MeshFilter[]
			{
				meshFilter,
			};

			//Text:
			(GameObject labelGameObject, TextMeshPro textRenderer) = Helper.textObjectMono("FlatKey: TextDecoration");
			textRenderer.fontSizeMin = 0.01f;
			textRenderer.enableAutoSizing = true;
			textRenderer.horizontalAlignment = HorizontalAlignmentOptions.Center;
			textRenderer.verticalAlignment = VerticalAlignmentOptions.Middle;

			return new Decoration[]
			{
				//The primary decoration, which is the key-cap:
				new Decoration()
				{
					LocalPosition = new Vector3(0, 0.1f + 0.3f * 0.2f, 0),
					LocalRotation = Quaternion.identity,
					DecorationObject = keycapGameObject,
					AutoSetupColliders = true,
					IncludeInModels = true,
				},
				//The label decoration, which is the text/description:
				new Decoration()
				{
					LocalPosition = new Vector3(
						(Data.sizeX - 1) * 0.15f,
						0.1f + 0.3f * 0.2f + 0.001f,
						(Data.sizeZ - 1) * 0.15f
					),
					LocalRotation = Quaternion.AngleAxis(90, Vector3.right),
					DecorationObject = labelGameObject,
					AutoSetupColliders = false,
					IncludeInModels = true,
				},
			};
		}

		//Resizeable interface things:

		public int SizeX
		{
			get => Data.sizeX;
			set => Data.sizeX = value;
		}

		public int MinX => 1;
		public int MaxX => 10;
		public float GridIntervalX => 1;

		public int SizeZ
		{
			get => Data.sizeZ;
			set => Data.sizeZ = value;
		}

		public int MinZ => 1;
		public int MaxZ => 10;
		public float GridIntervalZ => 1;

		//Custom data interface things:

		protected override void SetDataDefaultValues()
		{
			Data.KeyDown = false;
			Data.BoundInput = 2;
			Data.KeyColor = new Color24(85, 85, 85);
			Data.KeyLabelColor = new Color24(229, 229, 229);
			Data.sizeX = 1;
			Data.sizeZ = 1;
			Data.label = null; //Yes 'null' is the default - means no overwrite.
		}

		//Pressable button interface:

		public void MousePressDown()
		{
			manuallyPressed = true;
			QueueFrameUpdate();
		}

		public void MousePressUp()
		{
			manuallyPressed = false;
			QueueFrameUpdate();
		}

		public IReadOnlyList<MeshFilter> OutlineWhenInteractableLookedAt { get; private set; }

		//Weird stuff for color:

		public Color24 Color
		{
			get => this.Data.KeyColor;
			set => this.Data.KeyColor = value;
		}

		public string ColorsFileKey => "Interactables";
		public float MinColorValue => 0.0f;
	}
}
