using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Money : NetworkBehaviour
{
    private Player player;

	[HideInInspector]
	public GameObject DrawButton;

	[HideInInspector]
	public TextMeshProUGUI moneyAmountText;
	public TextMeshProUGUI costText;

	public int CurrentMoney;
    public int CurrentCost;

	void Start()
    {
        DrawButton = GameObject.FindGameObjectWithTag("DrawButton");

        moneyAmountText = GetComponentInChildren<TextMeshProUGUI>();

        GameObject costObject = GameObject.FindGameObjectWithTag("CostText");
        costText = costObject.GetComponent<TextMeshProUGUI>();

        CurrentMoney = 0;
        CurrentCost = 2;

        ShowMoney(CurrentMoney);
    }

    public void ShowMoney(int money) 
    {
        CurrentMoney = money;
        moneyAmountText.text = money.ToString();
    }

    public void SpendMoney() 
    {
        CurrentMoney -= CurrentCost;
        ShowMoney(CurrentMoney);

		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		player = networkIdentity.GetComponent<Player>();

		player.CmdUpdateMoney(CurrentMoney);
	}

    public void ChangeCost(int newCost) 
    {
        CurrentCost = newCost;
        costText.text = CurrentCost.ToString();
    }
}
