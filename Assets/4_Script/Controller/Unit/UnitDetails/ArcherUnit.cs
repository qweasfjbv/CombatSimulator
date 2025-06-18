using Combat.Interfaces;
using Combat.Manager;
using Combat.VFX;
using UnityEngine;

namespace Combat.Controller
{
	public class ArcherUnit : UnitController
	{
		public override void Attack(Transform target)
		{
			if (target == null || target.GetComponent<IDamagable>() == null) return;

			target.GetComponent<IDamagable>().ReserveDamage(unitData.DamageType, unitData.StatsByLevel[0].AttackPower, unitData.AttackDelay);
			TrailBase tb = PoolingManager.Instance.Spawn(Utils.ProjectileType.Arrow, unitData.AttackDelay).GetComponent<TrailBase>();
			tb.SetTrail(transform.position, target, unitData.AttackDelay);
		}

		public override bool IsSameUnit(int unitId, int level)
		{
			return unitId == 1;
		}

		protected override void ExecuteSkill(Transform[] targets, int targetCounts)
		{

		}

	}
}
