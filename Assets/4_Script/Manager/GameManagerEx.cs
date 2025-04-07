using Defense.Controller;
using Defense.Debugger;
using IUtil;
using System.Collections;
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

		[SerializeField] private GameObject towerPrefab1;
		[SerializeField] private GameObject towerPrefab2;

		[SerializeField, Range(0.5f, 3.0f)]
		private float timeScale = 1.0f;

		private void Awake()
		{
			Init();
		}

		private void Update()
		{
			Time.timeScale = timeScale;
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				SpawnTowers();
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				StartTest();
			}
		}

		[Button()]
		private void SpawnTowers()
		{
			for (int i = 5; i < 6; i++)
			{
				TowerController tc = Instantiate(towerPrefab1, new Vector3(5, 0, 5 * (i+1)), Quaternion.identity).GetComponent<TowerController>();
				tc = Instantiate(towerPrefab2, new Vector3(-5, 0, 5 * (i+1)), Quaternion.identity).GetComponent<TowerController>();
			}
		}


		[Button()]
		private void StartTest()
		{
			GetComponent<WaypointsDrawer>().DrawWaypoints(Managers.Resource.GetRouteData(0).Waypoints);
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
