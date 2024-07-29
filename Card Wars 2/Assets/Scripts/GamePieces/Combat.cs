using Mirror;
using UnityEngine;

public class Combat : NetworkBehaviour
{
	[Server]
	public void Altercation(CreatureCard attackingCard, CreatureCard defendingCard) { RpcBattle(attackingCard, defendingCard); }
	
	[ClientRpc]
	public void RpcBattle(CreatureCard attackingCard, CreatureCard defendingCard) 
	{
		attackingCard.Animate.Hop();

		if (defendingCard == null)
		{
			if (attackingCard.isOwned) 
			{
				Player player = NetworkClient.localPlayer.GetComponent<Player>();
				player.CmdShowStats(player.Money + attackingCard.AttackStat, "money");
			}
		}
		else 
		{
			defendingCard.TakeDamage(attackingCard.AttackStat);
		}
	}
}
