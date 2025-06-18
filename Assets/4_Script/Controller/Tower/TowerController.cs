using Combat.Manager;
using Combat.Utils;
using UnityEngine;

namespace Combat.Controller
{
	public class TowerController : MonoBehaviour
	{
		private void OnEnable()
		{
			PoolingManager.Instance.SpawnParticle(ParticleType.Build, transform.position);
		}

	}
}