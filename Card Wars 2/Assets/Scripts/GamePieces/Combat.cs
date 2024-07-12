using Mirror;

public class Combat : NetworkBehaviour
{
	[Server] // called from client via command
	public void Altercation(CreatureCard attackingCard, CreatureLand defendingLand) 
	{
		CreatureCard acrossCard = defendingLand.CurrentCard.GetComponent<CreatureCard>();

		DealDamage(attackingCard, acrossCard);
	}

	[ClientRpc]
	public void DealDamage(CreatureCard attackingCard, CreatureCard defendingCard) 
	{
		if (defendingCard == null)
		{
			//defendingCard: get owner of this card, and give the guap to them, mabye relative to damage dealth
		}
		else 
		{
			defendingCard.TakeDamage(attackingCard.AttackStat);
		}
	}

	public void HitEmptyLand()
	{

	}
}

