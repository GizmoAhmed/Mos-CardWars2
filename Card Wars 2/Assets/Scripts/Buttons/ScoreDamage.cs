using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDamage : NetworkBehaviour
{
	public void DealDamage()
	{
		Player player = NetworkClient.localPlayer.GetComponent<Player>();

		Debug.Log("This player is trying to deal " + player.Score + " damage");

		player.CmdDealDamage(player);
	}
}
