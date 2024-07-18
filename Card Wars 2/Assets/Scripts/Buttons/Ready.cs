using Mirror;

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
