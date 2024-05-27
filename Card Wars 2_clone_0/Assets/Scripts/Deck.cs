using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Deck : NetworkBehaviour
{
	public PlayerManager playerManager;

	public TextMeshProUGUI DeckSizeText;
	public List<GameObject> CardsInDeck;

	void Start()
	{
		DeckSizeText = GetComponentInChildren<TextMeshProUGUI>();
	}

	public void DrawCard()
	{
		Debug.Log("Deck Clicked...");

		NetworkIdentity networkIdentity = NetworkClient.connection.identity;

		playerManager = networkIdentity.GetComponent<PlayerManager>();

		playerManager.CmdDrawCard(CardsInDeck);
	}

	void Update()
	{
		DeckSizeText.text = CardsInDeck.Count.ToString();
	}
}
