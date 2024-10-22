using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class BuildingCard : Card
{
	[Header("Building Stats")]
	public int currentTime;
	public int maxTime;

	[Tooltip("Allows for building to last an extra turn")]
	public bool firstTurn;

	new void Start()
	{
		base.Start();
		maxTime = currentTime;
		transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = currentTime.ToString();
		landTag = "BuildLand";
		firstTurn = true;
	}

	[ClientRpc]
	public override void RpcDecay()
	{
		if (firstTurn) 
		{
			firstTurn = false;
			return;
		}

		currentTime -= 1;

		if (currentTime <= 0)
		{
			FillTime();
			base.Discard();
		}
		else
		{
			transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = currentTime.ToString();
		}
	}

	public void FillTime()
	{
		currentTime = maxTime;
		transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = currentTime.ToString();
	}
}
