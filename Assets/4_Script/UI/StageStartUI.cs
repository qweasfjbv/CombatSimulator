using DG.Tweening;
using TMPro;
using UnityEngine;

public class StageStartUI : MonoBehaviour
{
	[SerializeField] private RectTransform background1;
	[SerializeField] private RectTransform background2;
	[SerializeField] private TextMeshProUGUI stageText;

	[SerializeField] private float moveDuration = 0.5f;
	[SerializeField] private float stayDuration = 0.5f;

	private float background1StartPos;
	private float background2StartPos;

	private void Awake()
	{
		background1.gameObject.SetActive(false);
		background2.gameObject.SetActive(false);
		stageText.gameObject.SetActive(false);
	}

	public void OnStartStage(int idx)
	{
		stageText.text = $"WAVE {idx}";

		float screenWidth = Screen.width;
		float screenHeight = Screen.height;

		background1StartPos = -screenWidth*4;
		background2StartPos = screenWidth*4;

		background1.anchoredPosition = new Vector2(background1StartPos, 0f);
		background2.anchoredPosition = new Vector2(background2StartPos, 0f);
		stageText.rectTransform.anchoredPosition = background1.anchoredPosition;


		background1.gameObject.SetActive(true);
		background2.gameObject.SetActive(true);
		stageText.gameObject.SetActive(true);

		Sequence sequence = DOTween.Sequence();

		sequence.Append(background1.DOAnchorPosX(0, moveDuration).SetEase(Ease.OutQuad));
		sequence.Join(background2.DOAnchorPosX(0, moveDuration).SetEase(Ease.OutQuad));
		sequence.Join(stageText.rectTransform.DOAnchorPosX(0, moveDuration).SetEase(Ease.OutQuad));

		sequence.AppendInterval(stayDuration);

		sequence.Append(background1.DOAnchorPosX(background1StartPos, moveDuration).SetEase(Ease.InQuad));
		sequence.Join(background2.DOAnchorPosX(background2StartPos, moveDuration).SetEase(Ease.InQuad));
		sequence.Join(stageText.rectTransform.DOAnchorPosX(background1StartPos, moveDuration).SetEase(Ease.InQuad));

		sequence.OnComplete(() =>
		{
			background1.gameObject.SetActive(false);
			background2.gameObject.SetActive(false);
			stageText.gameObject.SetActive(false);
		});

	}
}
