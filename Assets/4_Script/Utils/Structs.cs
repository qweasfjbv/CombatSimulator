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
	}


	[System.Serializable]
	public class ParticleEntry
	{
		public ParticleType Key;
		public GameObject Prefab;
		public int PoolSize;

		[HideInInspector] public Queue<ParticleSystem> Pool = new();
	}
}