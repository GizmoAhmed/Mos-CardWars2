using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class DrawCard : NetworkBehaviour
{
	public PlayerManager playerManager;

	public void Draw()
	{
		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		playerManager = networkIdentity.GetComponent<PlayerManager>();
		playerManager.CmdDrawCard();
	}
}
