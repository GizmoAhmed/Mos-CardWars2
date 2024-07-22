using Mirror;
using UnityEngine;

public class Combat : NetworkBehaviour
{
	[Server]
	public void Altercation(CreatureCard attackingCard, CreatureCard defendingCard) { RpcBattle(attackingCard, defendingCard); }
	
	[ClientRpc]
	public void RpcBattle(CreatureCard attackingCard, CreatureCard defendingCard) 
	{
		if (defendingCard == null)
		{
			Debug.Log(attackingCard.Name + " has no one across");
		}
		else 
		{
			defendingCard.TakeDamage(attackingCard.AttackStat);
		}
	}
}
