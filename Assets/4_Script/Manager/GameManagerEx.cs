using Defense.Controller;
using Defense.Debugger;
using Defense.Props;
using IUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defense.Manager
{
	public class GameManagerEx : MonoBehaviour
	{
		#region Singleton
		private static GameManagerEx s_instance;
		public static GameManagerEx Instance { get { return s_instance; } }

		public void Init()
		{
			if (s_instance == null)
			{
				s_instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
			else
			{
				Destroy(this.gameObject);
				return;
			}
		}
		#endregion

		[SerializeField] private GameObject personPrefab;
		[SerializeField] private int testCount;

		[SerializeField] private GameObject towerPrefab;
		[SerializeField] private GameObject magePrefab;
		[SerializeField] private GameObject archerPrefab;

		[SerializeField] private GameObject slotPrefab;

		[SerializeField, Range(0.5f, 3.0f)]
		private float timeScale = 1.0f;

		private List<PlacementSlot> slotList = new List<PlacementSlot>();

		private void Awake()
		{
			Init();
		}

		private void Update()
		{
			Time.timeScale = timeScale;
		}

		[Button]
		private void SpawnSlots()
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = -2; j <= 2; j++) 
				{
					slotList.Add(Instantiate(slotPrefab, new Vector3(5 * j, 0.01f, 5 * (i + 1)), Quaternion.Euler(90f, 0, 0)).GetComponent<PlacementSlot>());
				}
			}
		}

		[Button]
		private void SpawnMage()
		{
			for(int i=0; i<slotList.Count; i++)
			{
				if (!slotList[i].IsOccupied)
				{
					UnitController mageController = Instantiate(i%2 == 0 ? magePrefab : archerPrefab, slotList[i].transform.position, Quaternion.identity)
						.GetComponent<UnitController>();
					mageController.InitEnemy(0, i % 2 == 0 ? 2 : 1);
					slotList[i].SetUnit(mageController);
					return;
				}
			}
		}
		

		[Button]
		private void SpawnTowers()
		{
			for (int i = 5; i < 6; i++)
			{
				TowerController tc = Instantiate(towerPrefab, new Vector3(5, 0, 5 * (i+1)), Quaternion.identity).GetComponent<TowerController>();
				tc = Instantiate(towerPrefab, new Vector3(-5, 0, 5 * (i+1)), Quaternion.identity).GetComponent<TowerController>();
			}
		}


		[Button]
		private void StartTest()
		{
			//GetComponent<WaypointsDrawer>().DrawWaypoints(Managers.Resource.GetRouteData(0).Waypoints);
			StartCoroutine(SpawnCoroutine());
		}


		private IEnumerator SpawnCoroutine()
		{
			for (int i = 0; i < testCount; i++)
			{
				GameObject go = Instantiate(personPrefab, Managers.Resource.GetRouteData(0).SpawnPoint, Quaternion.identity);
				go.GetComponent<UnitController>().InitEnemy(0, 0);
				yield return new WaitForSeconds(0.01f);
			}
		}

	}
}
