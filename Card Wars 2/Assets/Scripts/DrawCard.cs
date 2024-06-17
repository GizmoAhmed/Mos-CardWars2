using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class DrawCard : NetworkBehaviour
{
	public PlayerManager playerManager;
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
			playerManager = networkIdentity.GetComponent<PlayerManager>();
			playerManager.CmdDrawCard();
		}

		
	}
}
