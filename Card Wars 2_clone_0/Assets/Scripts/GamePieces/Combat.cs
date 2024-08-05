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
			if (!attackingCard.isOwned) // attacking card is on the other side
			{
				Player player = NetworkClient.localPlayer.GetComponent<Player>();
				player.CmdShowStats(player.Health - attackingCard.AttackStat, "health");
			}
		}
		else 
		{
			defendingCard.TakeDamage(attackingCard.AttackStat);
		}
	}
}
