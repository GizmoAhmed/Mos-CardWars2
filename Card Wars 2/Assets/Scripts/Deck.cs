using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class Deck : NetworkBehaviour
{
	public PlayerManager playerManager;

	void Start()
	{

	}

	public void DrawCard()
	{
		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		playerManager = networkIdentity.GetComponent<PlayerManager>();
		playerManager.CmdDrawCard();
	}

	void Update()
	{

	}
}
