using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Ready : NetworkBehaviour
{
	public void ReadyUp()
	{
		Player player = NetworkClient.localPlayer.GetComponent<Player>();

		if (player != null)
		{
			player.CmdSetReady();
		}
	}
}
