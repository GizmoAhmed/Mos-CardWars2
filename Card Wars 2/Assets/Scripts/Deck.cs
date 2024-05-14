using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
	public TextMeshProUGUI DeckSizeText;
	public List<GameObject> CardsInDeck;
	public GameObject Hand;

	void Start()
	{
		DeckSizeText = GetComponentInChildren<TextMeshProUGUI>();

		DeckSizeText.text = CardsInDeck.Count.ToString();
	}

	public void DrawCard()
	{
		if (CardsInDeck.Count > 0)
		{
			int randomIndex = Random.Range(0, CardsInDeck.Count);

			GameObject drawnCard = Instantiate(CardsInDeck[randomIndex], new Vector2(0,0), Quaternion.identity);

			drawnCard.transform.SetParent(Hand.transform, false);

			CardsInDeck.RemoveAt(randomIndex);
		}
		
	}

	void Update()
	{
		DeckSizeText.text = CardsInDeck.Count.ToString();
	}
}
