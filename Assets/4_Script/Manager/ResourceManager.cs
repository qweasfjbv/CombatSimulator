
using UnityEngine;

namespace Defense.Manager
{
	public class ResourceManager
	{
		private string enemyDataPath = "Datas/ScriptableObjects/EnemyDatas/";
		private string routeDataPath = "Datas/ScriptableObjects/RouteDatas/";
		private string towerDataPath = "Datas/ScriptableObjects/TowerDatas/";


		private EnemyData[] enemyData;
		private RouteData[] routeData;
		private TowerData[] towerData;

		public void Init()
		{
			enemyData = Resources.LoadAll<EnemyData>(enemyDataPath);
			routeData = Resources.LoadAll<RouteData>(routeDataPath);
			towerData = Resources.LoadAll<TowerData>(towerDataPath);
		}

		public EnemyData GetEnemyData(int idx)
		{
			return enemyData[idx];
		}
		public RouteData GetRouteData(int idx)
		{
			return routeData[idx];
		}
		public TowerData GetTowerData(int idx)
		{
			return towerData[idx];
		}
	}
}
