using UnityEngine;
using DG.Tweening;
using Mirror;

public class CardAnimator : NetworkBehaviour
{
	private RectTransform rectTransform;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	public void Jiggle()
	{
		// Debug.Log(GetComponent<Card>().CardName + " jiggles");

		Sequence jiggleSequence = DOTween.Sequence();
		jiggleSequence.Append(rectTransform.DOShakeAnchorPos(0.7f, 0.9f, 10, 90, false, false));
	}

	public void Hop(bool reverse)
	{
		// Debug.Log(GetComponent<Card>().CardName + " hops");

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

	public void FadeOut(CanvasGroup canvasGroup, GameObject card, Player player, bool isOwned)
	{
		canvasGroup.DOFade(0, 1f).OnComplete(() => { player.discard.AddtoDiscard(card, isOwned); });
	}

}