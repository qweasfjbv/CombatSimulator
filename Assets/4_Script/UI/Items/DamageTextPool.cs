using Defense.Utils;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.Items
{
	public class DamageTextPool : MonoBehaviour
	{
		[SerializeField] private GameObject damageTextPrefab;
		[SerializeField] private Transform textParent;
		[SerializeField] private float damageFontSize;

		private Queue<Transform> pool = new Queue<Transform>();
		
		private RectTransform rt;
		private TextMeshProUGUI tmp;

		private Color tmpColor;
		private float fontSize;
		
		private string visualDamage;

		public void Show(Vector2 anchoredPos, float damage, DamageType damageType, HitResultType resultType)
		{
			Transform damageText;
			damageText = Get();
			rt = damageText.GetComponent<RectTransform>();
			tmp = damageText.GetComponent<TextMeshProUGUI>();

			DetermineTextVisual(damage, damageType, resultType, out tmpColor, out fontSize, out visualDamage);

			tmp.DOKill();
			rt.DOKill();

			rt.anchoredPosition = anchoredPos;
			tmp.text = visualDamage;
			tmp.color = tmpColor;
			tmp.fontSize = fontSize;
			damageText.gameObject.SetActive(true);
			rt.localScale = Vector3.one;

			Sequence seq = DOTween.Sequence();
			seq.Join(rt.DOAnchorPosY(rt.anchoredPosition.y + 50f, 1f))
				.Join(tmp.DOFade(0f, 1f))
				.OnComplete(() => Return(damageText));
		}

		private Transform Get()
		{
			return pool.Count > 0 ? pool.Dequeue() : Instantiate(damageTextPrefab, textParent).transform;
		}

		public void Return(Transform dt)
		{
			dt.gameObject.SetActive(false);
			pool.Enqueue(dt);
		}

		private void DetermineTextVisual(float damage, DamageType damageType, HitResultType resultType, 
			out Color color, out float fontSize, out string visualDamage)
		{
			color = Color.white;
			fontSize = damageFontSize;
			visualDamage = ((int)damage == 0 ? 1 : (int)damage).ToString();
		}
	}
}