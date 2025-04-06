using System.Collections.Generic;
using UnityEngine;

namespace Defense.Utils
{
	/// <summary>
	/// 레벨마다 달라지는 Stat 구조체
	/// </summary>
	[System.Serializable]
	public struct LevelStat
	{
		public float MaxHealth;
		public float AttackPower;
		public float DefensePower;
		public float CritProb;
	}


	[System.Serializable]
	public class ParticleEntry
	{
		public ParticleType Key;
		public GameObject Prefab;
		public int PoolSize;

		[HideInInspector] public Queue<ParticleSystem> Pool = new();
	}

	[System.Serializable]
	public class ProjectileEntry
	{
		public ProjectileType Key;
		public GameObject Prefab;
		public int PoolSize;

		[HideInInspector] public Queue<GameObject> Pool = new();
	}

	public struct HitResult
	{
		public HitResultType ResultType;
		public float FinalDamage;
		public DamageType DamageType;

		public HitResult(HitResultType resultType, float finalDamage, DamageType damageType)
		{
			ResultType = resultType;
			FinalDamage = finalDamage;
			DamageType = damageType;
		}
	}
}