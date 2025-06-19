using UnityEngine;
using System.Collections.Generic;

namespace Autobattler
{
	[CreateAssetMenu(fileName = "RouteData", menuName = "GameData/Route Data")]
	public class RouteData : ScriptableObject
	{
		[SerializeField] private Vector3 _spawnPoint;
		[SerializeField] private List<Vector3> _waypoints = new List<Vector3>();
		[SerializeField] private float _wayWidth;

		// ReadOnly Properties
		public Vector3 SpawnPoint => _spawnPoint;
		public List<Vector3> Waypoints => _waypoints;
		public float WayWidth => _wayWidth;
	}
}