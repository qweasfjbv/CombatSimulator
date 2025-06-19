using UnityEngine;

namespace Autobattler.UI
{
	public abstract class PopupUIBase : MonoBehaviour
	{
		public abstract void Show(Vector2 anchorPos);
		public abstract void Hide();
	}
}
