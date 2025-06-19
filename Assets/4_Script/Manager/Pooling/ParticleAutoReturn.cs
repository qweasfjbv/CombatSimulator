using Cysharp.Threading.Tasks;
using Autobattler.Manager;
using Autobattler.Utils;
using UnityEngine;

namespace Autobattler
{
	public class ParticleAutoReturn : MonoBehaviour
	{
		private ParticleSystem ps;
		public ParticleType originKey;

		private void Awake()
		{
			ps = GetComponent<ParticleSystem>();
		}

		private void OnEnable()
		{
			WaitForFinish().Forget();
		}

		private async UniTaskVoid WaitForFinish()
		{
			await UniTask.WaitUntil(() => !ps.IsAlive(true));

			if (PoolingManager.Instance != null)
			{
				PoolingManager.Instance.ReturnToParticlePool(originKey, ps);
			}
		}
	}
}
