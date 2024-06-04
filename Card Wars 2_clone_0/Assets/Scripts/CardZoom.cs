using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CardZoom : NetworkBehaviour
{
	public GameObject ZoomCardPrefab;

	public GameObject zoomedCard;
	public FloopExit floopScript;

	public GameObject Canvas;

	public GameObject deck;
	public Button deckButton;

	public Card cardScript;

	public bool ZoomedIn;

	public void Start()
	{
		Canvas = GameObject.Find("GameCanvas");

		deck = GameObject.Find("Deck1");

		deckButton = deck.GetComponent<Button>();

		cardScript = GetComponent<Card>();
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

		floopScript = zoomedCard.GetComponent<FloopExit>();

		// attach this card to the zoomedCards floop script
		floopScript.card = gameObject;

		zoomedCard.transform.SetParent(Canvas.transform, false);

		ZoomLock(true);

		ZoomedIn = true;
	}

	public void ZoomOut() 
	{
		Debug.Log("Zooming Out...");

		Destroy(zoomedCard);

		ZoomLock(false);

		ZoomedIn = false;
	}

	public void ZoomLock(bool set)
	{
		Card[] allCards = FindObjectsOfType<Card>();

		foreach (Card card in allCards)
		{
			card.isZoomLocked = set;
		}

		deckButton.interactable = !set;
	}

}