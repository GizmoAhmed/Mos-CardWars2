using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandSpacing : MonoBehaviour
{
	public GridLayoutGroup grid;
	public int cardCount;

	void Start()
	{
		grid = GetComponent<GridLayoutGroup>();
	}

	void Update()
	{
		cardCount = transform.childCount;
		grid.spacing = new Vector2(CalculateSpacing(cardCount), 0);
	}

	float CalculateSpacing(int cardCount)
	{
		// Calculate the spacing using the quadratic formula
		float spacing = -7.5f * cardCount * cardCount + 82.5f * cardCount - 187.5f;
		return spacing;
	}
}
