using Mirror;
using UnityEngine;

public class Combat : NetworkBehaviour
{
	[Server]
	public void Altercation(CreatureCard attackingCard, CreatureCard defendingCard) { RpcBattle(attackingCard, defendingCard); }
	
	[ClientRpc]
	public void RpcBattle(CreatureCard attackingCard, CreatureCard defendingCard) 
	{
		/// if attackers are owned, then that is the attacking player
		if (attackingCard.isOwned)
		{
			Debug.Log("attack cards are owned: this player found their battle cards");
		}
		else 
		{
			Debug.Log("attack cards are not owned: this player is defending");
		}

		if (defendingCard == null)
		{
			
		}
		else 
		{
			defendingCard.TakeDamage(attackingCard.AttackStat);
		}
	}
}
