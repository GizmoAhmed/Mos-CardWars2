using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class Combat : NetworkBehaviour
{
	[SyncVar] public List<GameObject> BattleReadyCards = new List<GameObject>();

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
        if (Player0.identity.GetComponent<Player>().myTurn)
            { DoBattle(Player0); }
        else 
            { DoBattle(Player1); }
	}

    [Server]
    public void DoBattle(NetworkConnectionToClient conn) 
    {
        Player thisPlayer = conn.identity.GetComponent<Player>();

        thisPlayer.RpcFindBattleCards();

        thisPlayer.myBattleReadyCards = BattleReadyCards;
	}
}
