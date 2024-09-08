using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDamage : NetworkBehaviour
{
	private Combat combat;

	void Start()
	{
		combat = FindAnyObjectByType<Combat>();
	}

	public void DealDamage()
	{
		Player player = NetworkClient.localPlayer.GetComponent<Player>();

		Debug.Log("This player is try to deal " + player.Score + " damage");

		combat.CmdDealDamage(player.Score);
	}
}
