using System.Collections;
using System.Collections.Generic;
using CardScripts;
using CardScripts.CardData;
using CardScripts.CardDisplays;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Lands;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class CharmArea : MiddleLand
{
    [SyncVar]
    public List<GameObject> InUseCharms = new List<GameObject>();

    public override void SetupNeighbors()
    {
        if (across == null) // then I guess lets just set it here
        {
            if (gameObject.name == "SpellGroup1")
            {
                across = GameObject.Find("SpellGroup2");
            }
            else if (gameObject.name == "SpellGroup2")
            {
                across = GameObject.Find("SpellGroup1");
            }
        }
    }
}
