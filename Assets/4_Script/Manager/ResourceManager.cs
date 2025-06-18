
using UnityEngine;

namespace Combat.Manager
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

		private UnitData[][] unitData;
		private UnitData[] playerData;
		private RouteData[] routeData;

		public void Init()
		{
			int rows = enemyDataPath.Length;
			unitData = new UnitData[rows][];

			for (int i = 0; i < rows; i++)
			{
				UnitData[] data = Resources.LoadAll<UnitData>(enemyDataPath[i]);
				if (data != null && data.Length > 0)
				{
					unitData[i] = data;
				}
				else
				{
					Debug.LogWarning($"Failed to load UnitData at path: {enemyDataPath[i]}");
					unitData[i] = new UnitData[0];
				}
			}
			playerData = Resources.LoadAll<UnitData>(playerDataPath);
			routeData = Resources.LoadAll<RouteData>(routeDataPath);
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
