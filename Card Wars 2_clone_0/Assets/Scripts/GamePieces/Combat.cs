using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class Combat : NetworkBehaviour
{
	// private Player player;

	private NetworkConnectionToClient Player0;
	private NetworkConnectionToClient Player1;

    // courtesy of SetUp.cs
    [Server]
    public void FindPlayers(NetworkConnectionToClient player0, NetworkConnectionToClient player1)
    {
        Player0 = player0;
        Player1 = player1;
	}

    
    // at the start of every attack.cs...
    [Server]
    public void InitializeCombat()
	{
        if (!Player1.identity.GetComponent<Player>().myTurn)
            {   DoBattle(Player0);
		        // DoBattle(Player1);
		    }
        else 
            {   DoBattle(Player1);
                // DoBattle(Player0); 
            }
	}

    [Server]
    public void DoBattle(NetworkConnectionToClient conn) 
    {

		Debug.Log($"Player {conn.connectionId} is finding battle cards");

		Player player = conn.identity.GetComponent<Player>();
		player.RpcFindBattleCards();
	}
}
