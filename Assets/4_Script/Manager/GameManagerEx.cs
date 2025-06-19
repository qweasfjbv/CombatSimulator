using Autobattler.Controller;
using Autobattler.Props;
using IUtil;
using System.Collections.Generic;
using UnityEngine;

namespace Autobattler.Manager
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

		[SerializeField] private GameObject slotPrefab;

		[SerializeField, Range(0.5f, 3.0f)]
		private float timeScale = 1.0f;

		private List<PlacementSlot> player1SlotList = new List<PlacementSlot>();
		private List<PlacementSlot> player2SlotList = new List<PlacementSlot>();

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
			int slotID = 0;
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					PlacementSlot slot = Instantiate(slotPrefab, new Vector3(5 * j, 0.01f, 15 + 5 * (i + 1)), Quaternion.Euler(90f, 0, 0)).GetComponent<PlacementSlot>();
					slot.InitSlot(SlotDir.Back, slotID++);
					player1SlotList.Add(slot);
				}
			}

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					PlacementSlot slot = Instantiate(slotPrefab, new Vector3(5 * j, 0.01f, -15 + 5 * (i + 1)), Quaternion.Euler(90f, 0, 0)).GetComponent<PlacementSlot>();
					slot.InitSlot(SlotDir.Front, slotID++);
					player2SlotList.Add(slot);
				}
			}
		}

		int randId = 0;
		[Button]
		private void SpawnMage()
		{
			int rand = UnityEngine.Random.Range(0, 2);
			List<PlacementSlot> playerSlotList = rand == 1 ? player1SlotList : player2SlotList;

			// 뭘 Spawn할지 결정
			int id = (randId++) % 4;
			int emptyIdx = -1;
			int sameIdx = -1;
			for(int i=0; i<playerSlotList.Count; i++)
			{
				if (playerSlotList[i].IsEmpty())
				{
					emptyIdx = i;
					continue;
				}

				if (playerSlotList[i].IsAbleToAdd(id, 0))
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

			UnitController newController = Instantiate(Managers.Resource.GetUnitPrefab(id, 0), playerSlotList[finalIndex].transform.position, Quaternion.identity)
				.GetComponent<UnitController>();
			newController.InitUnit(id);

			playerSlotList[finalIndex].AddUnit(newController);
		}

		// Change Input, hide slots
		[Button()]
		private void StartStage()
		{
			for (int i = 0; i < player1SlotList.Count; i++)
			{
				player1SlotList[i].OnStartStage();
			}
			for (int i = 0; i < player2SlotList.Count; i++)
			{
				player2SlotList[i].OnStartStage();
			}
		}

		// Change Input, Show slot, revive units
		[Button()]
		private void EndStage()
		{
			for (int i = 0; i < player1SlotList.Count; i++)
			{
				player1SlotList[i].OnEndStage();
			}
			for (int i = 0; i < player2SlotList.Count; i++)
			{
				player2SlotList[i].OnEndStage();
			}
		}


		private int currentWave = 0;
		public void OnGameStartButtonClicked()
		{

		}

	}
}
