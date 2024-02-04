using System;
using EccsLogicWorldAPI.Client.AccessHelpers;
using LogicWorld.Rendering.Components;
using RadioConnection.Shared;
using UnityEngine;

namespace RadioConnection.Client
{
	public class RadioComponent : ComponentClientCode<IRadioComponentData>
	{
		private static readonly Quaternion dataPegRotation = Quaternion.AngleAxis(90, Vector3.right) * Quaternion.AngleAxis(45, Vector3.up); //Rotate to side and then rotate around itself to be diagonal.
		
		//Fields to detect custom data changes:
		private uint lastAddressPegs;
		private uint lastDataPegs;
		private bool lastCompact;
		private bool lastFlip;
		
		protected override void SetDataDefaultValues()
		{
			Data.initialize();
		}
		
		//Called when the component gets created (which also means after its pegs have changed):
		protected override void Initialize()
		{
			fixPegPositions();
		}
		
		//Called when custom data changes (server or client dictated!):
		protected override void DataUpdate()
		{
			var pegChanged = lastAddressPegs != Data.addressPegs || lastDataPegs != Data.dataPegs;
			var nonPegChanged = lastCompact != Data.compactPegPlacement || lastFlip != Data.flipped;
			if(pegChanged || nonPegChanged)
			{
				fixPegPositions(); //Might be obsolete, if the peg amount has changed, will be called again when component is re-created. Better safe than sorry.
			}
			if(!pegChanged && nonPegChanged)
			{
				//If the component is being edited right now, its outline would change. Re-Initialize the edit window, so that the outline updates:
				EditWindowRefresher.updateEditWindow(Address);
			}
		}
		
		private void fixPegPositions()
		{
			if(InputCount != (Data.addressPegs + Data.dataPegs))
			{
				return; //Ohoh, this is bad. Try again later? Just wait till all data is adjusted properly.
			}
			//Save the data, to detect the next change:
			lastAddressPegs = Data.addressPegs;
			lastDataPegs = Data.dataPegs;
			lastCompact = Data.compactPegPlacement;
			lastFlip = Data.flipped;
			
			//Update the peg positions:
			var pegStep = 1f; //Distance to the next peg.
			var jitter = 0f; //Amount of how far data pegs jump up/down. By default not at all.
			var addressWidth = lastAddressPegs - (1f - 1f / 3f); //Amount of address pegs in blocks, but subtract the space left and right from all pegs (1 block - 1 peg).
			var dataWidth = lastDataPegs - (1f - 2f * 0.2616f); //Amount of data pegs in blocks, but subtract the space left and right from all pegs (1 block - 2 half diagonal bigger peg). 
			if(lastCompact) //If compact modify the values:
			{
				//Other compact stuff:
				pegStep /= 3f; //Now 3 pegs per block, so divide it by 3.
				jitter = 0.12f; //Just enough jitter, so that one can MultiWirePlace select all pegs in one group.
				//Divide by 3, as 3 pegs fit into one block:
				addressWidth = lastAddressPegs / 3f; //Now 3 pegs per block, so simply divide by 3. No fix required, as 3 pegs perfectly fit into one block.
				dataWidth = (lastDataPegs - 1) / 3f + 2f * 0.2616f; //Peg width is different from the distance between them! Hence take the distance from center of first and last peg and add one rotated bigger peg to it.
			}
			var widerPegWidth = addressWidth > dataWidth ? addressWidth : dataWidth; //For the block size, get the bigger peg group.
			var blockWidth = (float) Math.Ceiling(widerPegWidth); //Round up to get full blocks.
			
			//SET BLOCK:
			SetBlockScale(0, new Vector3(blockWidth, 1, 1));
			SetBlockPosition(0, new Vector3((blockWidth - 1) / 2f, 0.0f, 0.0f));
			
			//Constants for the height of pegs.
			const float startHeight = 0.4f;
			const float endHeight = 0.8f;
			const float deltaHeight = endHeight - startHeight;
			//Other helper:
			var rawStep = pegStep;
			
			// SET ADDRESS PEGS:
			//Subtract .5 to move the position to the start of the component.
			// Take the length of the block, subtract the width of the peg group. This results in the amount of unused space.
			// Divide that by two to get the point on the block where the pegs start.
			//Finally add the distance from start of pegs to the center of the first peg. Half a peg (1/6).
			var position = -.5f + (blockWidth - addressWidth + 1f / 3f) / 2f;
			var heightOffset = lastAddressPegs == 1 ? endHeight : startHeight; //If there is only one peg, make it tall instead of of the small start value. Looks better when changing the pegs.
			var heightStep = deltaHeight / (Data.addressPegs - 1); //The height change per peg. Amount of additions, hence the decrement.
			
			if(Data.flipped)
			{
				position += rawStep * (Data.addressPegs - 1); //If flipped, jump to the final placement position, by simply jumping the gap between pegs by the amount of gaps between gaps.
				pegStep *= -1; //This is only done once, here. Let the peg position grow backwards.
			}
			for(var i = 0; i < Data.addressPegs; i++)
			{
				SetInputPosition((byte) i, new Vector3(position, 1, 0));
				SetInputRotation((byte) i, Quaternion.identity);
				PegScale.SetInputHeight(WorldRenderer, this, i, heightOffset);
				heightOffset += heightStep;
				position += pegStep;
			}
			
			// SET DATA PEGS:
			//Subtract .5 to move the position to the start of the component.
			// Take the length of the block, subtract the width of the peg group. This results in the amount of unused space.
			// Divide that by two to get the point on the block where the pegs start.
			//Finally add the distance from start of pegs to the center of the first peg. Half a diagonal bigger peg (0.2616f).
			position = -.5f + (blockWidth - dataWidth) / 2f + 0.2616f;
			heightOffset = lastDataPegs == 1 ? endHeight : startHeight; //If there is only one peg, make it tall instead of of the small start value. Looks better when changing the pegs.
			heightStep = deltaHeight / (Data.dataPegs - 1); //The height change per peg. Amount of additions, hence the decrement.
			
			if(Data.flipped)
			{
				position += rawStep * (Data.dataPegs - 1); //If flipped, jump to the final placement position, by simply jumping the gap between pegs by the amount of gaps between gaps.
			}
			var down = true; //Helper to make jitter go up/down.
			for(var i = (int) Data.addressPegs; i < InputCount; i++)
			{
				SetInputPosition((byte) i, new Vector3(position, 0.5f + (down ? -jitter : jitter), 0.5f));
				SetInputRotation((byte) i, dataPegRotation);
				PegScale.SetInputScale(WorldRenderer, this, i, new Vector3(0.37f, heightOffset, 0.37f));
				heightOffset += heightStep;
				position += pegStep;
				down = !down;
			}
		}
	}
}
