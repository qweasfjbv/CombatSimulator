using Cysharp.Threading.Tasks.Triggers;
using Defense.Utils;
using UI.Items;
using UnityEngine;

namespace Defense.Manager
{
	public class GameUIManager : MonoBehaviour
	{
		private Canvas canvas;

		[SerializeField] private DamageTextPool damagePool;

		private void Awake()
		{
			canvas = GetComponent<Canvas>();
		}

		public void ShowDamage(Vector3 worldPos, float damage, DamageType damageType, HitResultType resultType)
		{
			Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(damagePool.GetComponent<RectTransform>(), screenPos, null, out localPoint);
			damagePool.ShowDamageText(localPoint, damage, damageType, resultType);
		}
	}
}
