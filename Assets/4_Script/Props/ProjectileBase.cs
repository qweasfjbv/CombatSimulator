using Autobattler.Controller;
using Autobattler.Utils;
using UnityEngine;

namespace Autobattler.Props { 
	public class ProjectileBase : MonoBehaviour
	{
		private UnitController target = null;
		private DamageType damageType = DamageType.Physics;
		private float damage = 0f;

		private float maxDuration = 0f;
		private float duration = 0f;

		private Quaternion targetRotation = Quaternion.identity;
		private Vector3 startPosition = Vector3.zero;

		/// <summary>
		/// duration동안 날라가서 damage를 입힘
		/// </summary>
		public void SetProjectileInfo(UnitController target, DamageType type, float damage, float duration)
		{
			this.target = target;
			damageType = type;
			this.damage = damage;
			maxDuration = this.duration = duration;

			startPosition = transform.position;
			targetRotation = transform.rotation;
		}

		public void OnUpdate()
		{
			if (target == null || target.gameObject == null) return;

			if (duration < 0f)
			{
				gameObject.SetActive(false);
			}

			targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
			transform.position = Vector3.Lerp(startPosition, target.transform.position, (maxDuration - duration) / maxDuration);

			duration -= Time.deltaTime;
		}

		private void OnDisable()
		{
			duration = 0f;
			damage = 0f;
			target = null;
		}

	}
}
