using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
	public Sprite CardFront;
	public Sprite CardBack;

	public void Flip()
	{
		Sprite currentSprite = gameObject.GetComponent<Image>().sprite;

		if (currentSprite == CardFront)
		{
			gameObject.GetComponent<Image>().sprite = CardBack;
			// Disable all children
			SetChildrenActive(false);
		}
		else
		{
			gameObject.GetComponent<Image>().sprite = CardFront;
			// Enable all children
			SetChildrenActive(true);
		}
	}

	private void SetChildrenActive(bool isActive)
	{
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(isActive);
		}
	}
}
