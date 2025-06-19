using Autobattler.Interfaces;
using UnityEngine;

namespace Autobattler.Controller
{
	public class ShieldUnit : UnitController
	{
		public override void Attack(Transform target)
		{
			if (target == null || target.GetComponent<IDamagable>() == null) return;

		}

		public override bool IsSameUnit(int unitId, int level)
		{
			return unitId == 3;
		}

		protected override void ExecuteSkill(Transform[] targets, int targetCounts)
		{

		}
	}
}
