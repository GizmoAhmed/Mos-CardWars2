using UnityEngine;
using DG.Tweening;
using Mirror;

public class CardAnimator : NetworkBehaviour
{
	private RectTransform rectTransform;


	void Awake() { rectTransform = GetComponent<RectTransform>(); }

	public void Jiggle()
	{
		Sequence jiggleSequence = DOTween.Sequence();
		jiggleSequence.Append(rectTransform.DOShakeAnchorPos(0.7f, 2f, 10, 90, false, false));
	}

	public void Hop(bool reverse)
	{
		Sequence hopSequence = DOTween.Sequence();

		if (reverse)
		{
			// Move up then down
			hopSequence.Append(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 15, 0.2f).SetEase(Ease.OutQuad));
			hopSequence.Append(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y, 0.2f).SetEase(Ease.InQuad));
		}
		else
		{
			// Move down then up
			hopSequence.Append(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y - 15, 0.2f).SetEase(Ease.OutQuad));
			hopSequence.Append(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y, 0.2f).SetEase(Ease.InQuad));
		}

		hopSequence.SetLoops(1, LoopType.Restart);
	}

}