using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CardHandler : NetworkBehaviour
{
    private GameObject PlayingCardGroup1;
    private GameObject PlayingCardGroup2;

    private void Start()
    {
        PlayingCardGroup1 = GameObject.Find("PlayingCardGroup1");
        PlayingCardGroup2 = GameObject.Find("PlayingCardGroup2");
        
        if (PlayingCardGroup1 == null || PlayingCardGroup2 == null)
        {
            Debug.LogError("PlayingCardGroup(s) were not set in editor");
        }
    }

    [Command]
    public void CmdDropCard(GameObject card, GameObject land)
    {
        RpcHandleCard(card, land);
    }

    /// <summary>
    /// Handles cards as they are being spawned or placed
    ///
    /// Cards can be:
    /// - Spawned from the deck list and put in hand
    /// - Placing them on the field 
    /// </summary>
    /// <param name="card"></param>
    /// <param name="land"></param>
    [ClientRpc]
    public void RpcHandleCard(GameObject card, GameObject land)
    {

        if (!land) // land was passed null, must be from deck, drawing card from deck
        {
            if (isOwned)
            {
                card.transform.SetParent(PlayingCardGroup1.transform, false);
            }
            else
            {
                card.transform.SetParent(PlayingCardGroup2.transform, false);
                
                // hide card in opps hand
                CardDisplay cardDisplay = card.GetComponent<CardDisplay>();
                cardDisplay.FlipCard(false);
            }
        }
        else // drop card on a land since land isn't null
        {
            // CardMovement.cs already makes sure card can actually be placed on this land
            if (isOwned)
            {
                land.GetComponent<MiddleLand>().AttachCard(card);
            }
            else
            {
                MiddleLand landScript = land.GetComponent<MiddleLand>();
                MiddleLand acrossLand = landScript.across.GetComponent<MiddleLand>();
                
                acrossLand.AttachCard(card);
            }
        }
    }
}
