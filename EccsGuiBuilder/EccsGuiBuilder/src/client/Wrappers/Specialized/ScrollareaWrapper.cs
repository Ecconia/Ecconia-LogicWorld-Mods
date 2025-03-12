using System;
using UnityEngine;

namespace EccsGuiBuilder.Client.Wrappers.Specialized
{
	public class ScrollareaWrapper : Wrapper<ScrollareaWrapper>
	{
		private SimpleWrapper content;
		
		public ScrollareaWrapper(GameObject gameObject) : base(gameObject)
		{
			content = new SimpleWrapper(gameObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject);
			children.Add(content);
		}
		
		public ScrollareaWrapper configureContent(Action<SimpleWrapper> configure)
		{
			configure(content);
			return this;
		}
	}
}
