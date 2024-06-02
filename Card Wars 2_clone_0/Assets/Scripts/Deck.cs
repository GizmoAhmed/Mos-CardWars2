using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class Deck : NetworkBehaviour
{
	public PlayerManager playerManager;
	public bool Clickable;

	void Start()
	{

	}

	public void DrawCard()
	{
		if (Clickable) 
		{
			NetworkIdentity networkIdentity = NetworkClient.connection.identity;
			playerManager = networkIdentity.GetComponent<PlayerManager>();
			playerManager.CmdDrawCard();
		}	
	}

	void Update()
	{

	}
}
