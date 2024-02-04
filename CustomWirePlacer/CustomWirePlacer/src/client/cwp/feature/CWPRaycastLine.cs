using LogicWorld.References;
using UnityEngine;

namespace CustomWirePlacer.Client.CWP.feature
{
	public class CWPRaycastLine
	{
		private readonly GameObject gameObject;
		
		private CWPGroupAxis axis;
		
		public CWPRaycastLine()
		{
			gameObject = new GameObject("CWP: Raycast-Guide");
			Object.DontDestroyOnLoad(gameObject); //TODO: Bind this to scene load or on activate.
			gameObject.SetActive(false);
			gameObject.AddComponent<MeshFilter>().mesh = Meshes.Cube;
			gameObject.AddComponent<MeshRenderer>().material = MaterialsCache.WorldObject(new Color(1.0f, 0.5f, 0)); //TODO: Other color.
		}
		
		public void setAxis(CWPGroupAxis axis)
		{
			reset();
			this.axis = axis;
			refresh();
		}
		
		public void reset()
		{
			gameObject.SetActive(false);
			axis = null;
		}
		
		public void onUpdate()
		{
			if(gameObject.activeSelf)
			{
				gameObject.transform.rotation *= Quaternion.AngleAxis(2.0f, Vector3.forward);
			}
		}
		
		public void refresh()
		{
			if(axis == null)
			{
				return;
			}
			if(!CWPSettings.showRaycastRay)
			{
				gameObject.SetActive(false);
				return;
			}
			var second = axis.secondPeg;
			if(second.IsEmpty())
			{
				gameObject.SetActive(false); //Then we do only have one peg - no ray.
				return;
			}
			
			var pFirst = CWPHelper.getRaycastPoint(axis.firstPeg);
			var pSecond = CWPHelper.getRaycastPoint(second);
			var ray = (pSecond - pFirst).normalized;
			
			var pStart = axis.backwards != null
				? CWPHelper.getPegRayCenter(axis.backwards[axis.backwards.Count - 1], pFirst, ray * -1)
				: pFirst;
			var pEnd = axis.forwards != null
				? CWPHelper.getPegRayCenter(axis.forwards[axis.forwards.Count - 1], pFirst, ray)
				: pSecond;
			
			gameObject.transform.rotation = Quaternion.LookRotation(ray, Vector3.up);
			gameObject.transform.position = (pStart + pEnd) / 2f;
			gameObject.transform.localScale = new Vector3(0.025f, 0.025f, Vector3.Distance(pStart, pEnd));
			gameObject.SetActive(true);
		}
	}
}
