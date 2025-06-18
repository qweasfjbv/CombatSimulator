using UnityEngine;

namespace Combat.VFX
{
	[RequireComponent(typeof(TrailRenderer))]
	public class TrailBase : MonoBehaviour
	{
		protected Vector3 startPoint;
		protected Vector3 endPoint;
		protected Transform target;
		protected float duration;
		protected float timer;
		protected TrailRenderer trail;

		protected bool isActive = false;

		private void Awake()
		{
			trail = GetComponent<TrailRenderer>();
			trail.enabled = false;
		}

		public virtual void SetTrail(Vector3 start, Transform target, float duration)
		{
			this.startPoint = start;
			this.target = target;
			this.duration = duration;
			transform.position = start;

			timer = 0f;
			trail.Clear();
			trail.enabled = true;
			isActive = true;
		}

		private void OnEnable()
		{

		}

		private void OnDisable()
		{
			trail.Clear();
			trail.enabled = false;
			isActive = false;
		}
	}
}
