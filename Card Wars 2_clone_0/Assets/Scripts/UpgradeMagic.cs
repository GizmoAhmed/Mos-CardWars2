using System;
using Mirror;
using UnityEngine;

public class UpgradeMagic : NetworkBehaviour
{
    private Player player;
    
    public void Upgrade()
    {
        Debug.Log("Upgrading Magic...");
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        player = networkIdentity.GetComponent<Player>();
        
        player.playerStats.CmdUpgradeMagic();
    }
}