using UnityEngine;
using Mirror;
using TMPro;
using System.Collections;

public class CreatureCard : Card
{
	// [Header("Base Stats")] // these can be set in each indivudal creature
	[SyncVar] private int Base_AttackStat;
	[SyncVar] private int Base_mDefenseStat;
	[SyncVar] private int Base_AbilityCost;

	[Header("Live Stats")]
	[SyncVar] public int AttackStat;
	[SyncVar] public int Defense;
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

		// given that the correct base stats are in the EDITOR, will prolly be changed later
		Base_AttackStat		= AttackStat;
		Base_mDefenseStat	= MaxDefense;
		Base_AbilityCost	= AbilityCost;

		Defense = MaxDefense;

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
		DefenseText.text = Defense.ToString() + "/" + MaxDefense.ToString();
	}

	public void TakeDamage(int damage) 
	{
		if (damage > 0) 
		{
			Animate.Jiggle();
		}
		
		Defense -= damage;

		if (Defense <= 0)
		{
			CardReset();
			base.Discard();
		}
		else 
		{
			UpdateStatText();
		}
	}

	public override void CardReset() 
	{
		// set all these stats back to the original before they were reset
		AttackStat	= Base_AttackStat;
		MaxDefense	= Base_mDefenseStat;
		Defense		= MaxDefense;
		AbilityCost = Base_AbilityCost;

		// remove charm here as well 

		UpdateStatText();
	}

	[Command]
	public void CmdBuff(int Amount, bool buff, string stat) { RpcBuff(Amount, buff, stat); }

	[ClientRpc]
	public void RpcBuff(int Amount, bool buff, string stat) { Buff(Amount, buff, stat); }

	public void Buff(int Amount, bool buff, string stat = "default") 
	{
		// ternary ==> variable = (condition) ? expressionTrue :  expressionFalse;
		switch (stat) 
		{
			case "attack":

				AttackStat += (buff) ? Amount : -Amount;

				if (AttackStat < 0) AttackStat = 0;

				break;
			case "defense":

				Defense += (buff) ? Amount : -Amount;
				MaxDefense += (buff) ? Amount : -Amount;

				if (Defense <= 0)
					{ Defense = MaxDefense = 1; } 

				break;
			case "ability":

				AbilityCost += (buff) ? -Amount : Amount;

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
