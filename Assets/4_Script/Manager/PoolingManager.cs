using Cysharp.Threading.Tasks;
using Defense.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Defense.Manager
{

	public class PoolingManager : MonoBehaviour
	{
		private static PoolingManager s_instance;
		public static PoolingManager Instance { get { return s_instance; } }


		[SerializeField] private List<ParticleEntry> particleEntries = new();
		[SerializeField] private List<ProjectileEntry> projectileEntries = new();

		private Dictionary<ParticleType, ParticleEntry> particleDict = new();
		private Dictionary<ProjectileType, ProjectileEntry> projectileDict = new();

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
				particleDict[entry.Key] = entry;

				for (int i = 0; i < entry.PoolSize; i++)
				{
					var ps = Instantiate(entry.Prefab, transform).GetComponent<ParticleSystem>();
					ps.gameObject.SetActive(false);

					var returner = ps.gameObject.AddComponent<ParticleAutoReturn>();
					returner.originKey = entry.Key;

					entry.Pool.Enqueue(ps);
				}
			}

			foreach (var entry in projectileEntries)
			{
				projectileDict[entry.Key] = entry;

				for (int i = 0; i < entry.PoolSize; i++)
				{
					GameObject go = Instantiate(entry.Prefab, transform);
					go.gameObject.SetActive(false);

					entry.Pool.Enqueue(go);
				}
			}

		}

		public void SpawnParticle(ParticleType key, Vector3 position)
		{
			if (!particleDict.TryGetValue(key, out var entry))
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
		public void ReturnToParticlePool(ParticleType key, ParticleSystem ps)
		{
			ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			ps.gameObject.SetActive(false);

			if (particleDict.TryGetValue(key, out var entry))
			{
				entry.Pool.Enqueue(ps);
			}
			else
			{
				Destroy(ps.gameObject);
			}
		}

		public GameObject Spawn(ProjectileType type, Vector3 position, Quaternion rotation, float autoReturnTime = -1f)
		{
			if (!projectileDict.ContainsKey(type))
			{
				Debug.LogWarning($"[PoolingManager] Pool for {type} doesn't exist!");
				return null;
			}

			GameObject obj = projectileDict[type].Pool.Count > 0 ?
				projectileDict[type].Pool.Dequeue() : Instantiate(projectileDict[type].Prefab, transform);

			obj.transform.SetPositionAndRotation(position, rotation);
			obj.SetActive(true);

			if (autoReturnTime > 0)
			{
				AutoReturnAfterDelay(type, obj, autoReturnTime).Forget();
			}

			return obj;
		}

		public void Return(ProjectileType type, GameObject obj)
		{
			obj.SetActive(false);
			projectileDict[type].Pool.Enqueue(obj);
		}

		private async UniTaskVoid AutoReturnAfterDelay(ProjectileType type, GameObject obj, float delay)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: this.GetCancellationTokenOnDestroy());
			if (obj != null && obj.activeInHierarchy)
			{
				Return(type, obj);
			}
		}
	}
}
