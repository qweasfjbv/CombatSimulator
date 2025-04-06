using UnityEngine;
using Defense.Utils;

namespace Defense
{
	[CreateAssetMenu(fileName = "TowerData", menuName = "GameData/Tower Data")]
	public class TowerData : ScriptableObject
	{
		[Header("Attack")]
		[SerializeField] private DamageType _damageType;
		[SerializeField] private float _towerRange;
		[SerializeField] private float _attackCooltime;
		[SerializeField] private float _attackDelay;

		[Header("Detect Settings")]
		[SerializeField] private SearchEnemyType _searchEnemyType;
		[SerializeField] private LayerMask _targetLayer;
		[SerializeField] private int _maxDetectCounts;

		// ReadOnly Properties
		public DamageType DamageType => _damageType;
		public float TowerRange => _towerRange;
		public float AttackCooltime => _attackCooltime;
		public float AttackDelay => _attackDelay;
		public SearchEnemyType SearchEnemyType => _searchEnemyType;
		public LayerMask TargetLayer => _targetLayer;
		public int MaxDetectCounts => _maxDetectCounts;
	}
}