using UnityEngine;
using Mirror;
using TMPro;


public class CreatureCard : Card
{
	[Header("Stats")]
	[SyncVar] public int AttackStat;
	[SyncVar] public int CurrentDefense;
	[SyncVar] public int MaxDefense;

	public enum Element
	{
		Forge,
		Spirit,
		Crystal,
		Tomb,
		School
	}

	[SyncVar] public Element myElement;

	private TextMeshProUGUI AttackText;
	private TextMeshProUGUI DefenseText;

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
		CurrentDefense -= damage;

		if (CurrentDefense <= 0)
		{
			Die();
		}

		DefenseText.text = CurrentDefense.ToString() + "/" + MaxDefense.ToString();
	}

	public void Die() 
	{
		Debug.Log(Name + " has just died");
	}

	public void Buff(int Amount, bool buff, string stat = "default") 
	{
		// if buff, increase stat by amount
	}
}
