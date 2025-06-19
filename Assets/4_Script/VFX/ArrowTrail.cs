using Autobattler.Utils;
using UnityEngine;
using Autobattler.VFX;

namespace Autobattler.Controller
{
	public class ArrowTrail : TrailBase
	{
		private void Update()
		{
			if (!isActive) return;
			if (target != null) endPoint = target.position;
			timer += Time.deltaTime;

			float t = timer / duration;

			if (t >= 1f)
			{
				gameObject.SetActive(false);
				return;
			}

			Vector3 currentPos = Calculation.CalculateBezierPoint(startPoint, endPoint, 6f, t);
			transform.position = currentPos;
		}
	}
}