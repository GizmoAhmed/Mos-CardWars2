using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spacing : MonoBehaviour
{
	public GridLayoutGroup grid;
	public int cardCount;

	[Header("ax^2 + bx + c")]
	public float a = 2.66071f;
	public float b = 54.9679f;
	public float c = 183f;

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
		float spacing = a * cardCount * cardCount - b * cardCount + c;
		return spacing;
	}
}
