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
        if (Player1.identity.GetComponent<Player>().myTurn)
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
		Player player = conn.identity.GetComponent<Player>();
		player.RpcFindBattleCards();

		if (BattleReadyCards.Count == 0) 
		{
			Debug.LogWarning("BattleReadyCards is empty for some reason");
		}

		foreach (GameObject cardOBJ in BattleReadyCards)
		{
			CreatureCard thisCard = cardOBJ.GetComponent<CreatureCard>();
			CreatureLand thisLand = thisCard.MyLand.GetComponent<CreatureLand>();

			CreatureLand acrossLand = thisLand._Across.GetComponent<CreatureLand>();

			if (acrossLand.CurrentCard == null)
			{
				player.RpcAttackAcross(thisCard, null);
			}
			else
			{
				CreatureCard acrossCard = acrossLand.CurrentCard.GetComponent<CreatureCard>();
				player.RpcAttackAcross(thisCard, acrossCard);
			}
		}
	}
}
