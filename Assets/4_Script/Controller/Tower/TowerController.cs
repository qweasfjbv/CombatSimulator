using Autobattler.Manager;
using Autobattler.Utils;
using UnityEngine;

namespace Autobattler.Controller
{
	public class TowerController : MonoBehaviour
	{
		private void OnEnable()
		{
			PoolingManager.Instance.SpawnParticle(ParticleType.Build, transform.position);
		}

	}
}