using UnityEngine;
using Mirror;

public class Upgrade : NetworkBehaviour
{
    private Player player;

    public void UpgradeMagic() 
    {
		player = NetworkClient.localPlayer.GetComponent<Player>();

		/*if (player.UpgradeCost <= player.Money)
		{
			player.CmdChangeStats(player.Money - player.UpgradeCost, "money"); // spend money

			player.CmdChangeStats(player.MaxMagic + 1, "max_magic");    // raise max magic (not current)

			player.UpgradeCost++;                                           // raise cost

			player.CmdChangeStats(player.UpgradeCost, "upgrade_cost");   // set cost
		}*/
	}
}
