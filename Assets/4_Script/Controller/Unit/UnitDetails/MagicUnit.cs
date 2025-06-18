using Combat.Interfaces;
using Combat.Manager;
using Combat.VFX;
using UnityEngine;

namespace Combat.Controller
{
	public class MagicUnit : UnitController
	{
		public override void Attack(Transform target)
		{
			if (target == null || target.GetComponent<IDamagable>() == null) return;
			target.GetComponent<IDamagable>().ReserveDamage(unitData.DamageType, unitData.StatsByLevel[0].AttackPower, unitData.AttackDelay);

			TrailBase tb = PoolingManager.Instance.Spawn(Utils.ProjectileType.Lightning, unitData.AttackDelay).GetComponent<TrailBase>();
			tb.SetTrail(transform.position, target, unitData.AttackDelay);
		}

		public override bool IsSameUnit(int unitId, int level)
		{
			return unitId == 2;
		}

		protected override void ExecuteSkill(Transform[] targets, int targetCounts)
		{
			// HACK - 스킬 테스트용
			if (targets == null || targets[0] == null) return;
			targets[0].GetComponent<IDamagable>().GetImmediateDamage(unitData.DamageType, unitData.StatsByLevel[0].AttackPower);
			PoolingManager.Instance.SpawnParticle(Utils.ParticleType.Lightning, targets[0].position);
		}
	}
}
