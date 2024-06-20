using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Magic : NetworkBehaviour
{
	[HideInInspector] public TextMeshProUGUI magicText;

	public int CurrentMagic;

	// Start is called before the first frame update
	void Start()
	{
		magicText = GetComponent<TextMeshProUGUI>();

		CurrentMagic = 2;
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
		Player player = networkIdentity.GetComponent<Player>();

		player.CmdUpdateMagic(CurrentMagic);
	}
}
