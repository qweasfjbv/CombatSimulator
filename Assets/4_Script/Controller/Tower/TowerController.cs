using Defense.Interfaces;
using Defense.Manager;
using Defense.Utils;
using UnityEngine;

namespace Defense.Controller
{
	public class TowerController : MonoBehaviour
	{
		[SerializeField] private TowerUnitController unitController;

		private void OnEnable()
		{
			ParticleManager.Instance.SpawnParticle(ParticleType.Build, transform.position);
		}

		public void InitTower(int towerId)
		{
			 unitController.InitTowerUnit(towerId);
		}
	}
}