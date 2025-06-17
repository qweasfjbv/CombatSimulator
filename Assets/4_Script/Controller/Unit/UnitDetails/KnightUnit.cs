using Defense.Interfaces;
using Defense.Manager;
using UnityEngine;

namespace Defense.Controller
{
	public class KnightUnit : UnitController
	{
		public override void Attack(Transform target)
		{
			if (target == null || target.GetComponent<IDamagable>() == null) return;

			target.GetComponent<IDamagable>().GetImmediateDamage(unitData.DamageType, unitData.StatsByLevel[0].AttackPower);
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