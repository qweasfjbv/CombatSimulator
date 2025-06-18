using Cysharp.Threading.Tasks;
using Combat.Manager;
using Combat.Utils;
using UnityEngine;

namespace Combat
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
