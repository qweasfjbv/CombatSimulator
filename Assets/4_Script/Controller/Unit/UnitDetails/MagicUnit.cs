using Defense.Interfaces;
using Defense.Manager;
using UnityEngine;

namespace Defense.Controller
{
	public class MagicUnit : UnitController
	{

		public override void Attack(Transform target)
		{
			if (target == null || target.GetComponent<IDamagable>() == null) return;

			target.GetComponent<IDamagable>().GetImmediateDamage(unitData.DamageType, unitData.StatsByLevel[0].AttackPower);
			PoolingManager.Instance.SpawnParticle(Utils.ParticleType.Lightning, target.position);
		}
		protected override void ExecuteSkill(Transform target)
		{
			// HACK
			Debug.Log("SKILL!");
		}
	}
}
