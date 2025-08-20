using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SpellArea : MiddleLand
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
    }

    public override bool ValidPlace(CardMovement card)
    {
        CardDataSO cardData = card.GetComponent<CardDisplay>().cardData;
        
        if (cardData.cardType == CardDataSO.CardType.Spell && gameObject.name.EndsWith("1"))
            return true;
        
        return false;
    }
}
