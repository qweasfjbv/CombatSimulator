using Autobattler.Utils;
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
		[SerializeField] private float fontSizeConstant; // 800f

		private Queue<Transform> pool = new Queue<Transform>();
		
		private RectTransform rt;
		private TextMeshProUGUI tmp;

		private Color fontColor;
		private Color outlineColor;
		private float fontSize;
		
		private string visualDamage;

		public void ShowDamageText(Vector2 anchoredPos, float damage, DamageType damageType, HitResultType resultType)
		{
			Vector2 offset = (new Vector2(Random.Range(-1f, 1f), Random.Range(0, 1f))) * 10f;
			Transform damageText;

			damageText = Get();
			rt = damageText.GetComponent<RectTransform>();
			tmp = damageText.GetComponent<TextMeshProUGUI>();

			Debug.Log(damage);
			DetermineTextVisual(damageType, resultType, out fontColor, out outlineColor, out fontSize);
			visualDamage = ((int)damage == 0 ? 1 : (int)damage).ToString();

			tmp.DOKill();
			rt.DOKill();

			rt.anchoredPosition = anchoredPos + offset;
			tmp.text = visualDamage;
			tmp.color = fontColor;
			tmp.outlineColor = outlineColor;
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

		private void DetermineTextVisual(DamageType damageType, HitResultType resultType, 
			out Color color, out Color outlineColor, out float fontSize)
		{
			color = Color.white;
			outlineColor = Color.black;
			fontSize = fontSizeConstant / Camera.main.orthographicSize;

			switch (resultType)
			{
				case HitResultType.Miss:
					outlineColor = Color.grey;
					break;
				case HitResultType.Normal:
					outlineColor = Color.black;
					break;
				case HitResultType.Critical:
					outlineColor = Color.red;
					break;
			}

			switch (damageType)
			{
				case DamageType.Physics:
					color = Color.white;
					break;
				case DamageType.Magic:
					color = new Color(42 / 255f, 234 / 255f, 198 / 255f);
					break;
				case DamageType.True:
					color = Color.black;
					break;
			}
		}
	}
}