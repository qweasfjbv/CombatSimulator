
using UnityEngine;

namespace Defense.Manager
{
	public class ResourceManager
	{
		private string[] enemyDataPath = new string[] {
			"Datas/ScriptableObjects/EnemyDatas/Warrior/",
			"Datas/ScriptableObjects/EnemyDatas/Archer/",
			"Datas/ScriptableObjects/EnemyDatas/Mage/",
			"Datas/ScriptableObjects/EnemyDatas/Shielder/"
		};

		private string routeDataPath = "Datas/ScriptableObjects/RouteDatas/";
		private string playerDataPath = "Datas/ScriptableObjects/PlayerDatas/";

		private UnitData[][] enemyData;
		private UnitData[] playerData;
		private RouteData[] routeData;

		public void Init()
		{
			int rows = enemyDataPath.Length;
			enemyData = new UnitData[rows][];

			for (int i = 0; i < rows; i++)
			{
				UnitData[] data = Resources.LoadAll<UnitData>(enemyDataPath[i]);
				if (data != null && data.Length > 0)
				{
					enemyData[i] = data;
				}
				else
				{
					Debug.LogWarning($"Failed to load UnitData at path: {enemyDataPath[i]}");
					enemyData[i] = new UnitData[0];
				}
			}
			playerData = Resources.LoadAll<UnitData>(playerDataPath);
			routeData = Resources.LoadAll<RouteData>(routeDataPath);
		}

		public UnitData GetEnemyData(int idx)
		{
			return enemyData[idx][0];
		}
		public UnitData GetPlayerData(int idx)
		{
			return playerData[idx];
		}
		public RouteData GetRouteData(int idx)
		{
			return routeData[idx];
		}
	}
}
