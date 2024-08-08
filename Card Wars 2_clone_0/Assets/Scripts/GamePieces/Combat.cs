using Mirror;
using UnityEngine;

public class Combat : NetworkBehaviour
{
	public float combatDelay = 1f;

	[Server]
	public void Altercation(CreatureCard attackingCard, CreatureCard defendingCard) { RpcBattle(attackingCard, defendingCard); }
	
	[ClientRpc]
	public void RpcBattle(CreatureCard attackingCard, CreatureCard defendingCard) 
	{
		attackingCard.Animate.Hop(attackingCard.isOwned);

		if (defendingCard == null)
		{
			if (attackingCard.isOwned) // if true, this player is attacking
			{
				Player player = NetworkClient.localPlayer.GetComponent<Player>();
				player.CmdShowStats(player.Score + attackingCard.AttackStat, "score");
			}
		}
		else 
		{
			defendingCard.TakeDamage(attackingCard.AttackStat);
			// if defending card's state is discard, that means it died. Give coins for it.
		}
	}
}
