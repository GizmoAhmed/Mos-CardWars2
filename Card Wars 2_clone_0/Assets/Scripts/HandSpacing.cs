using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandSpacing : MonoBehaviour
{
	public GridLayoutGroup grid;
	public int cardCount;
	public float xsquared = 2.66071f;
	public float xflat = 54.9679f;

	void Start()
	{
		grid = GetComponent<GridLayoutGroup>();
	}

	// Update is called once per frame
	void Update()
	{
		cardCount = transform.childCount;
		grid.spacing = new Vector2(CalculateSpacing(cardCount), 0);
	}

	float CalculateSpacing(int cardCount)
	{
		// cardCount is x in the quadratic equation
		// Calculate the spacing (y) using the quadratic formula y = 2.88393x^2 - 59.1161x + 201.83
		// float spacing = 2.88393f * cardCount * cardCount - 59.1161f * cardCount + 201.83f;
		float spacing = xsquared * cardCount * cardCount - xflat * cardCount + 183f;

		return spacing;
	}
}
