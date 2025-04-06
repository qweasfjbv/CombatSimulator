using Defense.Interfaces;
using UnityEngine;

namespace Defense.Controller
{
	public class ArcherTowerUnit : TowerUnitController
	{
		[Header("Archer")]
		[SerializeField] private GameObject arrowPrefab;

		public override void Attack(Transform target)
		{
			// TODO - 화살 풀링 필요
			target.GetComponent<IDamagable>().ReserveDamage(towerData.DamageType, 1f, towerData.AttackDelay);
			ArrowController ac = Instantiate(arrowPrefab, transform.position, Quaternion.identity).GetComponent<ArrowController>();
			ac.ShootArrow(target,6f, towerData.AttackDelay).Forget();
		}
	}
}
