using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Magic : NetworkBehaviour
{
	private PlayerManager playerManager;

	[HideInInspector]
	public TextMeshProUGUI magicText;

	public int CurrentMagic = 2;

	// Start is called before the first frame update
	void Start()
	{
		CurrentMagic = 2;
		magicText = GetComponent<TextMeshProUGUI>();

		ShowMagic(CurrentMagic);
	}

	public void ShowMagic(int magic)
	{
		CurrentMagic = magic;
		magicText.text = CurrentMagic.ToString();
	}

	public void SpendMagic(int cost)
	{
		CurrentMagic -= cost;
		magicText.text = CurrentMagic.ToString();

		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		playerManager = networkIdentity.GetComponent<PlayerManager>();

		playerManager.CmdUpdateMagic(CurrentMagic);
	}
}
