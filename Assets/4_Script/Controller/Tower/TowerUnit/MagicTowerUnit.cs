using Defense.Interfaces;
using Defense.Manager;
using UnityEngine;

namespace Defense.Controller
{
	public class MagicTowerUnit : TowerUnitController
	{
		public override void Attack(Transform target)
		{
			target.GetComponent<IDamagable>().GetImmediateDamage(towerData.DamageType, 1f);
			PoolingManager.Instance.SpawnParticle(Utils.ParticleType.Lightning, target.position);
		}
	}
}
