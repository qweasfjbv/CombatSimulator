using Cysharp.Threading.Tasks;
using Defense.Utils;
using UnityEngine;

namespace Defense.Controller
{
	public class ArrowController : MonoBehaviour
	{
		public async UniTaskVoid ShootArrow(Vector3 startPoint, Transform target, float height, float duration)
		{
			var trail = GetComponent<TrailRenderer>();

			trail.emitting = false;
			transform.position = startPoint;
			trail.Clear();
			trail.emitting = true;

			float time = 0f;
			Vector3 start = transform.position;
			Vector3 end = target.position;

			while (time < duration)
			{
				if (target != null) end = target.position;
				float t = time / duration;
				Vector3 position = Calculation.CalculateBezierPoint(start, end, height, t);
				transform.position = position;

				await UniTask.Yield(PlayerLoopTiming.Update);
				time += Time.deltaTime;
			}

			transform.position = end;
		}
	}
}