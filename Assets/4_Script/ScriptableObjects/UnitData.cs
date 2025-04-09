using Defense.Utils;
using UnityEngine;

namespace Defense
{
	[CreateAssetMenu(fileName = "EnemyData", menuName = "GameData/Enemy Data")]
	public class UnitData : ScriptableObject
	{
		[Header("Movement")]
		[SerializeField] private float _moveSpeed;
		[SerializeField] private float _rotationSpeed;

		[Header("Attack")]
		[SerializeField] private DamageType _damageType;
		[SerializeField] private float _searchRange;
		[SerializeField] private float _attackRange;
		[SerializeField] private float _attackCooltime;
		[SerializeField] private float _attackDelay;

		[Header("Knockback")]
		[SerializeField] private bool _useKnockback;
		[SerializeField] private float _knockbackDuration;

		[Header("Detect Settings")]
		[SerializeField] private LayerMask _targetLayer;
		[SerializeField] private int _maxDetectCounts;

		[Header("Durations")]
		[SerializeField] private float _deathAnimDuration;
		[SerializeField] private float _fadeOutDuration;
		[SerializeField] private float _skillDuration;

		[Header("Stats")]
		[SerializeField] private LevelStat[] _statsByLevel = new LevelStat[Constants.ENEMY_LEVEL_MAX];

		// ReadOnly Properties
		public float MoveSpeed => _moveSpeed;
		public float RotationSpeed => _rotationSpeed;
		public DamageType DamageType => _damageType;
		public float SearchRange => _searchRange;
		public float AttackRange => _attackRange;
		public float AttackCooltime => _attackCooltime;
		public float AttackDelay => _attackDelay;
		public bool UseKnockback => _useKnockback;
		public float KnockbackDuration => _knockbackDuration;
		public LayerMask TargetLayer => _targetLayer;
		public int MaxDetectCounts => _maxDetectCounts;
		public float DeathAnimDuration => _deathAnimDuration;
		public float FadeOutDuration => _fadeOutDuration;
		public float SkillDuration => _skillDuration;
		public LevelStat[] StatsByLevel => _statsByLevel;
	}
}