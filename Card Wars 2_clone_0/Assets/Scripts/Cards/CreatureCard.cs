using UnityEngine;
using Mirror;
using TMPro;

public class CreatureCard : Card
{
	[Header("Creature Stats")]
	[SyncVar] public int AttackStat;
	[SyncVar] public int CurrentDefense;
	[SyncVar] public int MaxDefense;

	private TextMeshProUGUI AttackText;
	private TextMeshProUGUI DefenseText;

	public enum Element { Forge, Spirit, Crystal, Tomb, School }

	[SyncVar] public Element myElement;

	new void Start()
	{
		base.Start();
		landTag = "CreatureLand";

		AttackText = transform.Find("Attack").GetComponent<TextMeshProUGUI>();
		DefenseText = transform.Find("Defense").GetComponent<TextMeshProUGUI>();

		AttackText.text = AttackStat.ToString();
		DefenseText.text = CurrentDefense.ToString() + "/" + MaxDefense.ToString();
	}

	public void TakeDamage(int damage) 
	{
		if (damage > 0) 
		{
			Animate.Jiggle();
		}
		
		CurrentDefense -= damage;

		if (CurrentDefense <= 0)
		{
			FullHeal();
			base.Discard();
		}
		else 
		{
			DefenseText.text = CurrentDefense.ToString() + "/" + MaxDefense.ToString();
		}
	}

	public void FullHeal() 
	{
		CurrentDefense = MaxDefense;
		DefenseText.text = CurrentDefense.ToString() + "/" + MaxDefense.ToString();
	}

	public void Buff(int Amount, bool buff, string stat = "default") 
	{
		// if buff, increase stat by amount
		// then check strength
	}

	[ClientRpc]
	public override void RpcDecay()
	{
		// does nothing
	}
}
