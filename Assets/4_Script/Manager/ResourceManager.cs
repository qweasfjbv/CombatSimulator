
using UnityEngine;

namespace Autobattler.Manager
{
	public class ResourceManager
	{
		private string[] enemyDataPath = new string[] {
			"Datas/ScriptableObjects/EnemyDatas/Warrior/",
			"Datas/ScriptableObjects/EnemyDatas/Archer/",
			"Datas/ScriptableObjects/EnemyDatas/Mage/",
			"Datas/ScriptableObjects/EnemyDatas/Shielder/"
		};
		private string[] enemyPrefabPath = new string[] {
			"Prefab/Unit/Warrior/",
			"Prefab/Unit/Archer/",
			"Prefab/Unit/Mage/",
			"Prefab/Unit/Shielder/"
		};

		private string routeDataPath = "Datas/ScriptableObjects/RouteDatas/";
		private string playerDataPath = "Datas/ScriptableObjects/PlayerDatas/";

		private UnitData[][] unitData;
		private UnitData[] playerData;
		private RouteData[] routeData;

		private GameObject[][] unitPrefabs;

		public void Init()
		{
			int rows = enemyDataPath.Length;
			unitData = new UnitData[rows][];
			unitPrefabs = new GameObject[rows][];

			for (int i = 0; i < rows; i++)
			{
				UnitData[] data = Resources.LoadAll<UnitData>(enemyDataPath[i]);
				GameObject[] prefabs = Resources.LoadAll<GameObject>(enemyPrefabPath[i]);

				if (data != null && data.Length > 0) unitData[i] = data;
				if (prefabs != null && prefabs.Length > 0) unitPrefabs[i] = prefabs;

				if (data == null || prefabs == null)
				{
					Debug.LogWarning("ResourceManager - There is no resources to load!");
				}
			}

			playerData = Resources.LoadAll<UnitData>(playerDataPath);
			routeData = Resources.LoadAll<RouteData>(routeDataPath);
		}

		public GameObject GetUnitPrefab(int index, int level)
		{
			return unitPrefabs[index][level];
		}

		public UnitData GetUnitData(int idx)
		{
			return unitData[idx][0];
		}

		public RouteData GetRouteData(int idx)
		{
			return routeData[idx];
		}
	}
}
