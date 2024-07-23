using UnityEngine;
using Mirror;

public class Upgrade : NetworkBehaviour
{
    private Player player;

    public void UpgradeMagic() 
    {
		player = NetworkClient.localPlayer.GetComponent<Player>();

		/*NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		player = networkIdentity.GetComponent<Player>();*/

		if (player.UpgradeCost <= player.Money)
		{
			player.CmdShowStats(player.Money - player.UpgradeCost, "money"); // spend money

			player.CmdShowStats(player.MaxMagic + 1, "max_magic");    // raise max magic (not current)

			player.UpgradeCost++;                                           // raise cost

			player.CmdShowStats(player.UpgradeCost, "upgrade_cost");   // set cost
		}
	}
}
