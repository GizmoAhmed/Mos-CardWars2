using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandSpacing : MonoBehaviour
{
	public GridLayoutGroup grid;
	public float multiplier = 1.0f;
	public int cardCount;

	void Start()
	{
		grid = GetComponent<GridLayoutGroup>();
	}

	// Update is called once per frame
	void Update()
	{
		cardCount = transform.childCount;

		grid.spacing = new Vector2(45f - cardCount * multiplier, 0);
	}
}
