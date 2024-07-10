using Mirror;
using UnityEngine;

public class Combat : NetworkBehaviour
{
    // at the start of every attack.cs...
    [Server]
    public void InitializeCombat()
	{
		FindAnyObjectByType<Turns>().ManageTurn(null, "disableBoth");

		if (FindAnyObjectByType<GameManager>().Player0.identity.GetComponent<Player>().myTurn)
        {
			Battle(FindAnyObjectByType<GameManager>().Player0);
			//DoBattle(FindAnyObjectByType<GameManager>().Player1); 
		}
		else 
        {
			Battle(FindAnyObjectByType<GameManager>().Player1);
			// DoBattle(FindAnyObjectByType<GameManager>().Player0); 
		}
	}

    [Server]
    public void Battle(NetworkConnectionToClient conn) 
    {
		Debug.Log($"Player {conn.connectionId} is finding battle cards");

		Player player = conn.identity.GetComponent<Player>();
		player.RpcFindBattleCards();
	}
}
