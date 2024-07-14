using Mirror;
using UnityEngine;

public class Combat : NetworkBehaviour
{
	[Server] // called from client via command
	public void Altercation(CreatureCard attackingCard, CreatureLand defendingLand) 
	{
		Debug.Log("Server: Altercation called");

		if (defendingLand.CurrentCard == null)
		{
			RpcDealDamage(attackingCard, null);
		}
		else 
		{
			CreatureCard acrossCard = defendingLand.CurrentCard.GetComponent<CreatureCard>();

			if (attackingCard == acrossCard) 
			{
				Debug.LogError(attackingCard.Name + " is attacking itself...");
				return;
			}

			RpcDealDamage(attackingCard, acrossCard);
		}
	}

	[ClientRpc]
	public void RpcDealDamage(CreatureCard attackingCard, CreatureCard defendingCard)
	{
		Debug.Log("Client is dealing damage");

		if (defendingCard == null)
		{
			Debug.Log(attackingCard.Name + " has no one across");
		}
		else 
		{
			Debug.Log(attackingCard.Name + " hit " + defendingCard.Name + " with " + 
				attackingCard.AttackStat.ToString() + " damage");

			defendingCard.TakeDamage(attackingCard.AttackStat);
		}
	}
}

