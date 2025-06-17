using Defense.Props;
using UnityEngine;

namespace Defense.UI
{
	public class SlotPopup : PopupUIBase
	{
		private PlacementSlot currentSlot = null;

		public override void Show(Vector2 anchorPos)
		{
			gameObject.SetActive(true);
			GetComponent<RectTransform>().anchoredPosition = anchorPos;
		}

		public void SetInfo(PlacementSlot slot)
		{
			currentSlot = slot;
		}

		public override void Hide()
		{
			gameObject.SetActive(false);
		}

	}
}
