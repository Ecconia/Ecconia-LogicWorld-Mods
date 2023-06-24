using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using EccsLogicWorldAPI.Client.AccessHelpers;
using UnityEngine;

namespace EccsGuiBuilder.Client.Wrappers.RootWrappers
{
	public class CanvasWrapper : RootWrapper<CanvasWrapper>
	{
		public CanvasWrapper(GameObject gameObject, string name) : base(gameObject)
		{
			gameObject.name = name;
		}
		
		public void build()
		{
			Assigner.assign(this, gameObject);
			Initializer.recursivelyInitialize(gameObject); //Initialize everything
		}
	}
}
