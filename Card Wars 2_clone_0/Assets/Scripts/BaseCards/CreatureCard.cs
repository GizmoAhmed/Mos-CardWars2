using UnityEngine;
using Mirror;
using TMPro;
using System.Collections;

public class CreatureCard : Card
{
	[Header("Creature Stats")]
	[SyncVar] public int AttackStat;
	[SyncVar] public int CurrentDefense;
	[SyncVar] public int MaxDefense;
	[SyncVar] public int AbilityCost;

	private TextMeshProUGUI AttackText;
	private TextMeshProUGUI DefenseText;
	private TextMeshProUGUI AbilityText;

	public enum Element { Forge, Spirit, Crystal, Tomb, School }

	[SyncVar] public Element myElement;

	new void Start()
	{
		base.Start();
		landTag = "CreatureLand";

		AttackText = transform.Find("Attack").GetComponent<TextMeshProUGUI>();
		DefenseText = transform.Find("Defense").GetComponent<TextMeshProUGUI>();

		UpdateStatText();
	}

	void UpdateStatText() 
	{
		if (AttackText == null || DefenseText == null)
		{
			Debug.LogError("Attack Text = " + AttackText + "\nDefense Text = " + DefenseText);
			return;
		}

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
			UpdateStatText();
			//DefenseText.text = CurrentDefense.ToString() + "/" + MaxDefense.ToString();
		}
	}

	public void FullHeal() 
	{
		CurrentDefense = MaxDefense;
		DefenseText.text = CurrentDefense.ToString() + "/" + MaxDefense.ToString();
	}

	public void Buff(int Amount, bool buff, string stat = "default") 
	{

		// variable = (condition) ? expressionTrue :  expressionFalse;
		switch (stat) 
		{
			case "attack":

				AttackStat += buff ? Amount : -Amount;

				if (AttackStat < 0) AttackStat = 0;

				break;
			case "defense":

				CurrentDefense += buff ? Amount : -Amount;
				MaxDefense += buff ? Amount : -Amount;

				if (CurrentDefense <= 0)
					{ CurrentDefense = MaxDefense = 1; } 

				break;
			case "ability":

				AbilityCost += buff ? -Amount : Amount;

				break;
			default:
				Debug.LogError("stat: " + stat + " isn't recognized");
				break;
		}

		UpdateStatText();
	}

	[ClientRpc]
	public override void RpcDecay()
	{
		// does nothing
	}
}
