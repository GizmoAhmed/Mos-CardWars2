using System.Collections;
using System.Collections.Generic;
using CardScripts;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class CharmArea : MiddleLand
{
    List<GameObject> spells = new List<GameObject>();

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

    public override void AttachCard(GameObject card)
    {
        card.transform.SetParent(transform, true);
        
        spells.Add(card);

        CardDisplay cardDisplay = card.GetComponent<CardDisplay>();
        
        cardDisplay.FlipCard(true);
        
        CardStats cardStats = card.GetComponent<CardStats>();
        
        if (cardStats?.thisCardOwner != null)
        {
            cardStats.thisCardOwner.UseMagic(cardStats.magic);
        }
    }

    public override bool ValidPlace(CardMovement cardMove)
    {
        CardStats cardStats = cardMove.GetComponent<CardStats>();
        
        Player cardOwner = cardStats.thisCardOwner.gameObject.GetComponent<Player>();

        // if not your turn, you can't place a card anywhere
        if (cardOwner != null &&
            cardOwner.GetComponent<Player>().myTurn == false)
        {
            return false;
        }

        if (cardOwner.playerStats.currentMagic < 1)
        {
            if (cardStats.magic != 0) // you should still be able to place stuff that cost zero 
            {
                return false; 
            }
        }
        
        CardDataSO cardData = cardMove.GetComponent<CardDisplay>().cardData;
        
        // only Carhms can be placed in charm area
        if (cardData.cardType == CardDataSO.CardType.Charm && gameObject.name.EndsWith("1")) // SpellGroup1
            return true;
        
        return false;
    }
}
