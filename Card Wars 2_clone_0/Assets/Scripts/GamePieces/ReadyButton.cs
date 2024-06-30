using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ReadyButton : NetworkBehaviour
{
	public void ReadyUp()
	{
		Player player = NetworkClient.localPlayer.GetComponent<Player>();

		if (player != null)
		{
			player.CmdSetReady();
			// player.EnablePlayer(false);
		}
	}
}
