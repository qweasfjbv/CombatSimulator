using UnityEngine;

namespace Autobattler.VFX
{
	public class LightningTrail : TrailBase
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
			float noiseStrength = 1f;
			float verticalNoiseStrength = 0.05f;

			Vector3 direction = (endPoint - startPoint).normalized;
			Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
			Vector3 up = Vector3.up;

			Vector3 currentPos = Vector3.Lerp(startPoint, endPoint, t);

			float sideJitter = (Mathf.PerlinNoise(Time.time * 20f, 0f) - 0.5f) * noiseStrength;
			float verticalJitter = (Mathf.PerlinNoise(0f, Time.time * 20f) - 0.5f) * verticalNoiseStrength;

			currentPos += right * sideJitter;
			currentPos += up * verticalJitter;

			transform.position = currentPos;
		}
	}

}