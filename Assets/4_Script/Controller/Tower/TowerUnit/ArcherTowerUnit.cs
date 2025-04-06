using Defense.Interfaces;
using Defense.Manager;
using UnityEngine;

namespace Defense.Controller
{
	public class ArcherTowerUnit : TowerUnitController
	{
		public override void Attack(Transform target)
		{
			target.GetComponent<IDamagable>().ReserveDamage(towerData.DamageType, 1f, towerData.AttackDelay);
			ArrowController ac = PoolingManager.Instance.Spawn(Utils.ProjectileType.Arrow, transform.position, Quaternion.identity, towerData.AttackDelay).GetComponent<ArrowController>();
			ac.ShootArrow(target, 6f, towerData.AttackDelay).Forget();
		}
	}
}
