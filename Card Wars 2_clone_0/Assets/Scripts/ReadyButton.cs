using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ReadyButton : NetworkBehaviour
{
	private Player player;

	public void ReadyUp()
	{
		Debug.Log("Clicked Ready...");

		player = FindAnyObjectByType<Player>();
		player.CmdSetReady();
	}
}
