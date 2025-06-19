using System.Collections.Generic;
using UnityEngine;

namespace Autobattler.Debugger
{
	public class WaypointsDrawer : MonoBehaviour
	{
		[SerializeField] private GameObject linePrefab;
		private GameObject lineGameObject;

		public void DrawWaypoints(List<Vector3> waypoints)
		{
#if UNITY_EDITOR

			if (lineGameObject != null) Destroy(lineGameObject);

			lineGameObject = Instantiate(linePrefab);
			LineRenderer renderer= lineGameObject.GetComponent<LineRenderer>();

			renderer.positionCount = waypoints.Count;
			for (int i = 0; i < waypoints.Count; i++) 
			{
				renderer.SetPosition(i, waypoints[i]);
			}
#endif
		}
	}
}