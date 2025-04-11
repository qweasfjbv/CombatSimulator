using Defense.Props;
using Defense.UI;
using Defense.Utils;
using System.Collections.Generic;
using System.Linq;
using UI.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Defense.Manager
{
	public class GameUIManager : MonoBehaviour
	{
		private Canvas canvas;

		[SerializeField] private Button backgroundPanel;
		[SerializeField] private DamageTextPool damagePool;
		[SerializeField] private PopupUIBase slotPopup;

		Stack<PopupUIBase> popupStack = new();

		private void Awake()
		{
			canvas = GetComponent<Canvas>();

			backgroundPanel.onClick.AddListener(() =>
			{
				CloseTopPopup();
			});
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
			RectTransformUtility.ScreenPointToLocalPointInRectangle(slotPopup.GetComponent<RectTransform>(), screenPos, null, out localPoint);
			slotPopup.Show(localPoint);
			(slotPopup as SlotPopup).SetInfo(slot);

			popupStack.Push(slotPopup);
		}

	}
}
