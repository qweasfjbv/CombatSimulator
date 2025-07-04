using Autobattler.Interfaces;
using UnityEngine;

namespace Autobattler.Controller
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
			return unitId == 0;
		}

		protected override void ExecuteSkill(Transform[] targets, int targetCounts)
		{

		}

	}
}