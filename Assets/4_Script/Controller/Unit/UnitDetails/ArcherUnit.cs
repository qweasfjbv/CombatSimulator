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

		public override bool IsSameUnit(int unitId, int level)
		{
			return unitId == 1;
		}

		protected override void ExecuteSkill(Transform target)
		{

		}

	}
}
