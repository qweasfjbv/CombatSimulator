using Defense.Controller;
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
			// 뭘 Spawn할지 결정
			int id = Random.Range(1, 3);
			int emptyIdx = -1;
			int sameIdx = -1;
			for(int i=0; i<slotList.Count; i++)
			{
				if (slotList[i].IsEmpty())
				{
					emptyIdx = i;
					continue;
				}

				if (slotList[i].IsAbleToAdd(id, 0))
				{
					sameIdx = i;
				}
			}

			int finalIndex = -1;

			if (emptyIdx >= 0) finalIndex = emptyIdx;
			if (sameIdx >= 0) finalIndex = sameIdx;

			if(finalIndex < 0)
			{
				Debug.Log("필드 가득참!!");
				return;
			}

			UnitController newController = Instantiate(id == 2 ? magePrefab : archerPrefab, slotList[finalIndex].transform.position, Quaternion.identity)
				.GetComponent<UnitController>();
			newController.InitEnemy(0, id);
			slotList[finalIndex].AddUnit(newController);
		}

		[SerializeField] private int towerIndex = 0;		
		
		[Button(nameof(towerIndex))]
		private void SpawnTowers(int index)
		{
			Instantiate(towerPrefab, slotList[index].transform.position, Quaternion.identity).GetComponent<TowerController>();
			slotList[index].SetTower();
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
