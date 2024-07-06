using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CreatureCard : Card
{
	[Header("Stats")]
	[SyncVar] public int Attack;
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

		AttackText.text = Attack.ToString();
		DefenseText.text = CurrentDefense.ToString() + "/" + MaxDefense.ToString();
	}

	/// pass a function to player that passes card, stat, and change
}
