
using Defense.Props;
using UnityEngine;

namespace Defense.UI
{
	public class SlotPopup : PopupUIBase
	{
		public override void Show(Vector2 anchorPos)
		{
			gameObject.SetActive(true);
			Debug.Log("SLOT POPUP SHOW");
		}

		public void SetInfo(PlacementSlot slot)
		{

		}

		public override void Hide()
		{
			Debug.Log("SLOT POPUP HIDE");
			gameObject.SetActive(false);
		}

	}
}
