using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CardZoom : NetworkBehaviour
{
	public GameObject Canvas;
	public GameObject ZoomCardPrefab;

	private GameObject zoomedCard;

	public bool ZoomedIn;

	public void Awake()
	{
		Canvas = GameObject.Find("GameCanvas");
	}

	public void ZoomIn()
	{
		// if already looking at a card
		if (ZoomedIn) 
		{
			return;
		}

		Debug.Log("Zooming In...");

		zoomedCard = Instantiate(ZoomCardPrefab, new Vector2(0,0), Quaternion.identity);

		zoomedCard.transform.SetParent(Canvas.transform, false);

		ZoomedIn = true;
	}

	public void ZoomOut() 
	{
		Destroy(zoomedCard);
		ZoomedIn = false;
	}
}