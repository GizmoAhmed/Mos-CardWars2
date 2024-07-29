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
		Debug.Log(gameObject.GetComponent<Card>().Name + " jiggles...");

		// Sequence for jiggle animation
		Sequence jiggleSequence = DOTween.Sequence();
		jiggleSequence.Append(rectTransform.DOShakeAnchorPos(2f, 1f, 10, 90, false, true));
		// jiggleSequence.SetLoops(-1, LoopType.Yoyo); // Loop indefinitely
	}

	public void Hop()
	{
		Debug.Log(gameObject.GetComponent<Card>().Name + " hops...");

		// Sequence for hop animation
		Sequence hopSequence = DOTween.Sequence();
		hopSequence.Append(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 30, 0.2f).SetEase(Ease.OutQuad)); // Move up
		hopSequence.Append(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y, 0.2f).SetEase(Ease.InQuad));  // Move down
		hopSequence.SetLoops(3, LoopType.Restart); // Repeat 3 times
	}
}