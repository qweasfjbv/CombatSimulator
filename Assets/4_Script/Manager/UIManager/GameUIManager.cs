using Combat.Props;
using Combat.UI;
using Combat.Utils;
using IUtil;
using System.Collections.Generic;
using UI.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Combat.Manager
{
	public class GameUIManager : MonoBehaviour
	{
		private Canvas canvas;

		[SerializeField] private Button backgroundPanel;
		[SerializeField] private DamageTextPool damagePool;
		[SerializeField] private PopupUIBase slotPopup;
		[SerializeField] private StageStartUI stageStartUI;

		[SerializeField] private Button stageStartButton;

		Stack<PopupUIBase> popupStack = new();

		private void Awake()
		{
			canvas = GetComponent<Canvas>();

			backgroundPanel.onClick.AddListener(() =>
			{
				CloseTopPopup();
			});

			stageStartButton.onClick.AddListener(() =>
			{
				GameManagerEx.Instance.OnGameStartButtonClicked();
			});
		}

		public void OnStageStart(int stageIdx)
		{
			stageStartUI.OnStartStage(stageIdx);
		}

		private void CloseTopPopup()
		{
			if (popupStack.Count <= 0) return;

			PopupUIBase popup = popupStack.Pop();
			popup.Hide();
		}

		public void ShowDamage(Vector3 worldPos, float damage, DamageType damageType, HitResultType resultType)
		{
			Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(damagePool.GetComponent<RectTransform>(), screenPos, null, out localPoint);
			damagePool.ShowDamageText(localPoint, damage, damageType, resultType);
		}

		public void ShowSlotUI(Vector3 worldPos, PlacementSlot slot)
		{
			Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPos, null, out localPoint);
			slotPopup.Show(localPoint);
			(slotPopup as SlotPopup).SetInfo(slot);

			// TODO - popupstack 다지워야됨
			popupStack.Push(slotPopup);
		}

	}
}
