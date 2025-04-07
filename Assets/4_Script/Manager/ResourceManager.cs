
using UnityEngine;

namespace Defense.Manager
{
	public class ResourceManager
	{
		private string enemyDataPath = "Datas/ScriptableObjects/EnemyDatas/";
		private string routeDataPath = "Datas/ScriptableObjects/RouteDatas/";
		private string playerDataPath = "Datas/ScriptableObjects/PlayerDatas/";


		private UnitData[] enemyData;
		private UnitData[] playerData;
		private RouteData[] routeData;

		public void Init()
		{
			enemyData = Resources.LoadAll<UnitData>(enemyDataPath);
			playerData = Resources.LoadAll<UnitData>(playerDataPath);
			routeData = Resources.LoadAll<RouteData>(routeDataPath);
		}

		public UnitData GetEnemyData(int idx)
		{
			return enemyData[idx];
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
