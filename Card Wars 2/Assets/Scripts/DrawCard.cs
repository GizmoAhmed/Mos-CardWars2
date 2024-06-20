using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class DrawCard : NetworkBehaviour
{
	public GameManager game;
	public GameObject ThisMoney;

	public void Start()
	{
		ThisMoney = GameObject.Find("ThisMoney");
	}

	public void Draw()
	{
		Money moneyScript = ThisMoney.GetComponent<Money>();

		if (moneyScript.CurrentCost > moneyScript.CurrentMoney)
		{
			Debug.Log("To Expensive...");
		}
		else 
		{
			moneyScript.SpendMoney();

			NetworkIdentity networkIdentity = NetworkClient.connection.identity;
			Player player = networkIdentity.GetComponent<Player>();
			player.CmdDrawCard();
		}

		
	}
}
