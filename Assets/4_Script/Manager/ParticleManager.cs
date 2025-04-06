using Defense.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Defense.Manager
{

	public class ParticleManager : MonoBehaviour
	{
		private static ParticleManager s_instance;
		public static ParticleManager Instance { get { return s_instance; } }


		[SerializeField] private List<ParticleEntry> particleEntries = new();
		private Dictionary<ParticleType, ParticleEntry> entryMap = new();

		private void Awake()
		{
			Init();
			InitPool();
		}

		public void Init()
		{
			if (s_instance == null)
			{
				s_instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
			else
			{
				Destroy(this.gameObject);
				return;
			}
		}

		public void InitPool()
		{
			foreach (var entry in particleEntries)
			{
				entryMap[entry.Key] = entry;

				for (int i = 0; i < entry.PoolSize; i++)
				{
					var ps = Instantiate(entry.Prefab, transform).GetComponent<ParticleSystem>();
					ps.gameObject.SetActive(false);

					var returner = ps.gameObject.AddComponent<ParticleAutoReturn>();
					returner.originKey = entry.Key;

					entry.Pool.Enqueue(ps);
				}
			}
		}
		public void SpawnParticle(ParticleType key, Vector3 position)
		{
			if (!entryMap.TryGetValue(key, out var entry))
			{
				Debug.LogWarning($"[PoolingManager] No particle registered with key: {key}");
				return;
			}

			if (entry.Pool.Count > 0)
			{
				var ps = entry.Pool.Dequeue();
				ps.transform.position = position;
				ps.gameObject.SetActive(true);
				ps.Play();
			}
			else
			{
				Debug.LogWarning($"[PoolingManager] No available particle in pool for key: {key}");
			}
		}
		public void ReturnToPool(ParticleType key, ParticleSystem ps)
		{
			ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			ps.gameObject.SetActive(false);

			if (entryMap.TryGetValue(key, out var entry))
			{
				entry.Pool.Enqueue(ps);
			}
			else
			{
				Destroy(ps.gameObject);
			}
		}
	}
}
