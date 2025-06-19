using UnityEngine;

namespace Autobattler.Debugger
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
			lineRenderer.SetPosition(0, start);
			lineRenderer.SetPosition(1, end);

			lineRenderer.textureScale = new Vector2((int)(GetCameraPlaneDistance(start, end) * 2), 1f);
		}
		public float GetCameraPlaneDistance(Vector3 start, Vector3 end)
		{
			Vector3 camForward = Camera.main.transform.forward.normalized;
			Vector3 delta = end - start;

			Vector3 projection = Vector3.Project(delta, camForward);

			Vector3 flattened = delta - projection;

			return flattened.magnitude;
		}
	}
}