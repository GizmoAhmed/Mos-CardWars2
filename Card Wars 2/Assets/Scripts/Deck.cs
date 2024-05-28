using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		playerManager = networkIdentity.GetComponent<PlayerManager>();
		playerManager.CmdDrawCard();
	}

	void Update()
	{
		DeckSizeText.text = CardsInDeck.Count.ToString();
	}
}
