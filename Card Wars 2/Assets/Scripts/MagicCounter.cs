using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MagicCounter : NetworkBehaviour
{
    private PlayerStats playerStats;
    public int magicCounter;

    public void InitCounter()
    {
        playerStats = GetComponent<PlayerStats>();
        playerStats.currentMagic = magicCounter = 0;
        
        MiddleLand[] lands = FindObjectsOfType<MiddleLand>();

        foreach (var l in lands) 
        {
            if (l.CompareTag("Land1")) l.FindMagicCounter(this);
        }
    }

    public void AddMagic(int magic)
    {
        magicCounter += magic;
        playerStats.currentMagic = magic;
    }
}
