using Cysharp.Threading.Tasks;
using Defense.Utils;
using UnityEngine;

namespace Defense.Controller
{
	public class ArrowController : MonoBehaviour
	{
		public async UniTaskVoid ShootArrow(Transform target, float height, float duration)
		{
			float time = 0f;
			Vector3 start = transform.position;
			Vector3 end = target.position;

			while (time < duration)
			{
				if (target != null) end = target.position;
				float t = time / duration;
				Vector3 position = Calculation.CalculateBezierPoint(start, end, height, t);
				transform.position = position;

				await UniTask.Yield();
				time += Time.deltaTime;
			}

			transform.position = end;
		}
	}
}