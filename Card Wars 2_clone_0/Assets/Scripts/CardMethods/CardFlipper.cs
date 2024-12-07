using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
	public Sprite CardFront;
	public Sprite CardBack;

	public enum FaceState
	{
		FaceUp,
		FaceDown
	}

	public FaceState currentFace;

	public FaceState CurrentFace 
	{
		get { return currentFace; }
		set { currentFace = value; }
	}

	public void Flip()
	{
		Sprite currentSprite = gameObject.GetComponent<Image>().sprite;

		if (currentSprite == CardFront)
		{
			currentFace = FaceState.FaceDown;

			gameObject.GetComponent<Image>().sprite = CardBack;

			SetChildrenActive(false);
		}
		else
		{
			currentFace= FaceState.FaceUp;

			gameObject.GetComponent<Image>().sprite = CardFront;

			SetChildrenActive(true);
		}
	}

	private void SetChildrenActive(bool isActive)
	{
		foreach (Transform child in transform)
		{
			if (child.gameObject.name == "InfoSlide" || child.gameObject.name == "UseButton") { }
			else
			{
				child.gameObject.SetActive(isActive);
			}
			
		}
	}
}
