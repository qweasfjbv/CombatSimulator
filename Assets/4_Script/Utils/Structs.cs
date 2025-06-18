using System.Collections.Generic;
using UnityEngine;

namespace Combat.Utils
{
	/// <summary>
	/// 레벨마다 달라지는 Stat 구조체
	/// </summary>
	[System.Serializable]
	public struct LevelStat
	{
		public float MaxHealth;
		public float MaxMP;
		public float AttackPower;
		public float DefensePower;
		public float CritProb;
	}

	[System.Serializable]
	public class ParticleEntry
	{
		[HideInInspector] public Queue<ParticleSystem> Pool = new();

		public GameObject Prefab;
		public int PoolSize;
		public ParticleType Key;
	}

	[System.Serializable]
	public class ProjectileEntry
	{
		[HideInInspector] public Queue<GameObject> Pool = new();

		public GameObject Prefab;
		public int PoolSize;
		public ProjectileType Key;
	}

	public struct HitResult
	{
		public float FinalDamage;
		public HitResultType ResultType;
		public DamageType DamageType;

		public HitResult(HitResultType resultType, float finalDamage, DamageType damageType)
		{
			ResultType = resultType;
			FinalDamage = finalDamage;
			DamageType = damageType;
		}
	}

	public struct DamageLog
	{
		public int atkSlotId;
		public int defSlotId;
		public int atkDamage;
	}

}