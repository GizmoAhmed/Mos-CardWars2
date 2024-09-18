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

		if (defendingCard != null)
		{
			defendingCard.TakeDamage(attackingCard.AttackStat);
		}

		if (attackingCard.isOwned) // if true, local player is attacking, if false, they're defending
		{
			Player attackingPlayer = NetworkClient.localPlayer.GetComponent<Player>();

			attackingPlayer.CmdChangeStats(attackingPlayer.Score + attackingCard.AttackStat, "score");
		}
	}

}
