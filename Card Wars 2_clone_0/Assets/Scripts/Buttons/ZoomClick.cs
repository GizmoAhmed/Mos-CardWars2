using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ZoomClick : NetworkBehaviour
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

		deck = GameObject.Find("DrawButton");

		deckButton = deck.GetComponent<Button>();

		cardScript = GetComponent<Card>();
	}

	public void ZoomIn()
	{
		// if already looking at a card
		if (ZoomedIn) { return; }

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