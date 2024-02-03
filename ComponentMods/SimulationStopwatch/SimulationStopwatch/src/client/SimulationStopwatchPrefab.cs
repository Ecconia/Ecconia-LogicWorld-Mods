using System;
using JimmysUnityUtilities;
using LogicWorld.Interfaces;
using LogicWorld.Rendering.Dynamics;
using LogicWorld.SharedCode.Components;
using Vector3 = UnityEngine.Vector3;

namespace SimulationStopwatch.Client
{
	public class SimulationStopwatchPrefab : PrefabVariantInfo
	{
		public override string ComponentTextID => "SimulationStopwatch.SimulationStopwatch";
		
		public override PrefabVariantIdentifier GetDefaultComponentVariant()
		{
			return new PrefabVariantIdentifier(2, 0);
		}
		
		public override ComponentVariant GenerateVariant(PrefabVariantIdentifier identifier)
		{
			if(identifier.OutputCount != 0)
			{
				throw new Exception("Attempted to create FileDump with output pegs. This component does not support output pegs. Fix your save or mod!");
			}
			if(identifier.InputCount != 2)
			{
				throw new Exception("Attempted to create FileDump with wrong input peg count. This component is not designed for that. Fix your save or mod!");
			}
			
			return new ComponentVariant()
			{
				VariantPrefab = new Prefab()
				{
					Blocks = new Block[]
					{
						new Block()
						{
							RawColor = new Color24(100, 100, 100),
						},
					},
					Inputs = new ComponentInput[]
					{
						// 0 - Starting peg:
						new ComponentInput()
						{
							Position = new Vector3(0, 1, 0),
							Length = 0.5f,
						},
						// 1 - Stopping peg:
						new ComponentInput()
						{
							Position = new Vector3(0, .5f, -.5f),
							Rotation = new Vector3(-90, 0, 0),
							Length = .4f,
						},
					},
				},
			};
		}
	}
}
