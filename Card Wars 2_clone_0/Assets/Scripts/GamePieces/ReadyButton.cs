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
		player = NetworkClient.localPlayer.GetComponent<Player>();

		if (player != null)
		{
			player.CmdSetReady();
			// player.EnablePlayer(false);
		}
	}
}
