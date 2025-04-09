using UnityEngine;

namespace Defense.Debugger
{
    [RequireComponent(typeof(LineRenderer))]
    public class DottedLineRenderer : MonoBehaviour
    {
        private LineRenderer lineRenderer;

		private void Awake()
		{
			lineRenderer = GetComponent<LineRenderer>();
		}

		public void DrawDottedLine(Vector3 start,  Vector3 end)
		{
			float distance = Vector3.Distance(start, end);
			start.y = .05f;
			end.y = .05f;
			lineRenderer.SetPosition(0, start);
			lineRenderer.SetPosition(1, end);
			lineRenderer.textureScale = new Vector2((int)(distance * 2), 1f);
		}
	}
}