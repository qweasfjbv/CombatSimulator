using Defense.Interfaces;
using Defense.Manager;
using UnityEngine;

namespace Defense.Controller
{
	public class ArcherUnit : UnitController
	{
		public override void Attack(Transform target)
		{
			if (target == null || target.GetComponent<IDamagable>() == null) return;

			target.GetComponent<IDamagable>().ReserveDamage(unitData.DamageType, unitData.StatsByLevel[0].AttackPower, unitData.AttackDelay);
			ArrowController ac = PoolingManager.Instance.Spawn(Utils.ProjectileType.Arrow, unitData.AttackDelay + 2 * Time.deltaTime).GetComponent<ArrowController>();
			ac.ShootArrow(transform.position, target, 6f, unitData.AttackDelay).Forget();
		}

		protected override void ExecuteSkill(Transform target)
		{

		}

	}
}
