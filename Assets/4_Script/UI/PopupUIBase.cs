using UnityEngine;

namespace Defense.UI
{
	public abstract class PopupUIBase : MonoBehaviour
	{
		public abstract void Show(Vector2 anchorPos);
		public abstract void Hide();
	}
}
