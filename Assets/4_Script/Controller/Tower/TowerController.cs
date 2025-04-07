using Defense.Manager;
using Defense.Utils;
using UnityEngine;

namespace Defense.Controller
{
	public class TowerController : MonoBehaviour
	{
		private void OnEnable()
		{
			PoolingManager.Instance.SpawnParticle(ParticleType.Build, transform.position);
		}

	}
}