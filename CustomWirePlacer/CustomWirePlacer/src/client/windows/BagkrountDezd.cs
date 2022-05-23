using UnityEngine;
using UnityEngine.UI;

namespace CustomWirePlacer.Client.Windows
{
	public class BagkrountDezd : Graphic
	{
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear(); //Unity has some very bizarre API, that does not clean up properly. Hence we have to do that.
			AddQuad(vh, rectTransform.rect.min, rectTransform.rect.max, Vector2.zero, Vector2.one);
		}

		private void AddQuad(VertexHelper vh, Vector2 lowerleft, Vector2 upperright, Vector2 lowerleftUV, Vector2 upperrightUV)
		{
			int index = vh.currentVertCount;
			vh.AddVert(lowerleft, color, lowerleftUV);
			vh.AddVert(new Vector2(lowerleft.x, upperright.y), color, new Vector2(lowerleftUV.x, upperrightUV.y));
			vh.AddVert(upperright, color, upperrightUV);
			vh.AddVert(new Vector2(upperright.x, lowerleft.y), color, new Vector2(upperrightUV.x, lowerleftUV.y));
			vh.AddTriangle(index, index + 1, index + 2);
			vh.AddTriangle(index, index + 2, index + 3);
		}
	}
}
