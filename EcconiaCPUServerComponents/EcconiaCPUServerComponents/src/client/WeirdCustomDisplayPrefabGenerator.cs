using System;
using LogicAPI.Data;
using LogicWorld.References;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode;
using LogicWorld.SharedCode.Components;
using UnityEngine;

namespace EcconiaCPUServerComponents.Client
{
	//I do not need to put this code here, I could put it in the SUCC file.
	// But then I think I would go crazy. So here it stays.
	public class WeirdCustomDisplayPrefabGenerator : DynamicPrefabGenerator<(int, int)>
	{
		//MUST BE AN EVEN NUMBER, OR CODE AND MODEL BREAK! Actually it is used everywhere now, do not change.
		private const int DisplaySideLength = 32;
		// (32 'Selector' + 32 'Invert Selector') * 2 'X/Y' + 1 'Data'
		private const int PegAmount = DisplaySideLength * 4 + 1;
		
		//This is how far a panel displays stands out of a board. This display should be aligned with them:
		private const float PanelDisplayOffset = 1f / 3f; //Third of a square.
		//Other offsets:
		private const float GeneralZOffset = .25f + PanelDisplayOffset; //.25 is a quarter square.
		private const float PegZOffset = -1f + GeneralZOffset; //-1f, because the pegs start one block behind the component.
		private const float BlockZOffset = GeneralZOffset;
		//The distance from the pegs to the pixel center on axis.
		private const float PegPairOffset = 1f / 3f; //Is not aligned to the squares, but looks better.

		protected override (int, int) GetIdentifierFor(ComponentData componentData)
			=> (componentData.InputCount, componentData.OutputCount);
		
		public override (int inputCount, int outputCount) GetDefaultPegCounts()
			=> (PegAmount + 1, 0); // +1 is the additional debug peg added at some point
		
		protected override Prefab GeneratePrefabFor((int, int) identifier)
		{
			var (inputCount, outputCount) = identifier;
			if (outputCount != 0)
			{
				throw new Exception("Attempted to create Ecconias WeirdCustomDisplay with output pegs. Fix your save (or mod)!");
			}
			var hasExtraPeg = inputCount == PegAmount + 1;
			if(!hasExtraPeg && inputCount != PegAmount)
			{
				throw new Exception("Attempted to create Ecconias WeirdCustomDisplay with unexpected input peg amount. Loading old save? Wrong mod version?");
			}
			
			//Blocks:
			var blocks = new Block[DisplaySideLength * DisplaySideLength + 1];
			{
				//Set Display faces:
				var index = 0;
				for(var y = 0; y < DisplaySideLength; y++)
				{
					for(var x = 0; x < DisplaySideLength; x++)
					{
						blocks[index++] = new Block
						{
							Scale = new Vector3(2.01f, 1f, 2.01f),
							Rotation = new Vector3(90f, 0f, 0f),
							Position = new Vector3(
								-.5f - x * 2f,
								1f + y * 2f,
								BlockZOffset
							),
							Mesh = Meshes.FlatQuad,
							RawColor = Colors.DisplayOff,
							ColliderData = new ColliderData
							{
								Type = ColliderType.None,
							},
							ShouldBeOutlined = false,
						};
					}
				}
				const float offset = 0.3f;
				//Currently debugging main block:
				blocks[index] = new Block()
				{
					Scale = new Vector3(DisplaySideLength * 2f, 1f - offset, DisplaySideLength * 2f),
					Rotation = new Vector3(-90f, 0f, 0f),
					Position = new Vector3(.5f - DisplaySideLength, DisplaySideLength, BlockZOffset - 1f / 2f - offset / 2f),
					Mesh = Meshes.Cube, //Meshes.BetterCube_OpenBottom, //Try a hovering display...
				};
			}
			
			//Inputs:
			var inputs = new ComponentInput[hasExtraPeg ? PegAmount + 1 : PegAmount];
			{
				var startX = -0.5f; //Going in the negative X axis, starting is at 0.5.
				var startY = +1.0f; //Going into positive Y axis, starting at 0.0.
				var middleX = +.5f - DisplaySideLength; //The first .5 are to get to the edge of the Display, to then go into the middle.
				var middleY = DisplaySideLength;
				
				var indexInvertX = DisplaySideLength * 1 - 1;
				var indexDataX = DisplaySideLength * 2 - 1;
				var indexInvertY = DisplaySideLength * 2;
				var indexDataY = DisplaySideLength * 3;
				//TBI: Is it worth it, to also have 4 start positions which include the offset?
				
				for(var i = 0; i < DisplaySideLength * 2; i += 2)
				{
					//Invert X:
					inputs[indexInvertX--] = new ComponentInput
					{
						Length = 0.6f,
						Rotation = new Vector3(-90f, 0, 0),
						Position = new Vector3(
							startX - i + PegPairOffset,
							middleY,
							PegZOffset
						),
					};
					//Data X:
					inputs[indexDataX--] = new ComponentInput
					{
						Rotation = new Vector3(-90f, 0, 0),
						Position = new Vector3(
							startX - i - PegPairOffset,
							middleY,
							PegZOffset
						),
					};
					//Invert Y:
					inputs[indexInvertY++] = new ComponentInput
					{
						Length = 0.6f,
						Rotation = new Vector3(-90f, 0, 0),
						Position = new Vector3(
							middleX,
							startY + i - PegPairOffset,
							PegZOffset
						),
					};
					//Data Y:
					inputs[indexDataY++] = new ComponentInput
					{
						Rotation = new Vector3(-90f, 0, 0),
						Position = new Vector3(
							middleX,
							startY + i + PegPairOffset,
							PegZOffset
						),
					};
				}
				
				//Data:
				var index = inputs.Length - (hasExtraPeg ? 2 : 1);
				inputs[index++] = new ComponentInput
				{
					Length = 1.1f, //Default is: 0.8
					Rotation = new Vector3(-90f, 0, 0),
					Position = new Vector3(middleX, middleY, PegZOffset),
					//WirePointHeight = 0.9f, //Default
					//Bottomless = true, //Default
					//StartOn = false, //Default
				};
				if(hasExtraPeg)
				{
					inputs[index] = new ComponentInput()
					{
						Length = 1.1f, //Default is: 0.8
						Rotation = new Vector3(-90f, 0, 0),
						Position = new Vector3(middleX - 5, middleY - 5, PegZOffset),
						//WirePointHeight = 0.9f, //Default
						//Bottomless = true, //Default
						//StartOn = false, //Default
					};
				}
			}
			return new Prefab
			{
				Blocks = blocks,
				Inputs = inputs,
			};
		}
	}
}
