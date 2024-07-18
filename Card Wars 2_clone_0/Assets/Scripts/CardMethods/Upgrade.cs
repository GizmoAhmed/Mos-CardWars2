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
			player.CmdShowConsumable(player.Money - player.UpgradeCost, "money"); // spend money

			player.CmdShowConsumable(player.MaxMagic + 1, "max_magic");    // raise max magic (not current)

			player.UpgradeCost++;                                           // raise cost

			player.CmdShowConsumable(player.UpgradeCost, "upgrade_cost");   // set cost
		}
	}
}
