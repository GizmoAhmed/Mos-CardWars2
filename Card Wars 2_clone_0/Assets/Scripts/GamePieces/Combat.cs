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
			if (!attackingCard.isOwned) // if true, local player is attacking, if false, they're defending
			{
				Player defendingPlayer = NetworkClient.localPlayer.GetComponent<Player>();
				defendingPlayer.CmdChangeStats(defendingPlayer.Health - attackingCard.AttackStat, "health");
			}
		}
		else 
		{
			defendingCard.TakeDamage(attackingCard.AttackStat);
			// if defending card's state is discard, that means it died. Give coins for it.
		}
	}
}
