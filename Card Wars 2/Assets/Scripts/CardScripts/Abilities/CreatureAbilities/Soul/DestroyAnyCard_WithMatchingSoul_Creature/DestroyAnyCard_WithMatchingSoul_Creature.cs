using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DestroyAnyCard_WithMatchingSoul_Creature", 
    menuName = "Abilities/Creature/DestroyAnyCard_WithMatchingSoul_Creature")]
public class DestroyAnyCard_WithMatchingSoul_Creature : ActiveAbilitySO
{
    // TODO: Add your ability parameters here
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get every card on field, yours or theirs

        List<CardStats> allCardsOnField = thisCard.Ext_GetAnyAndAllFieldCards();

        if (allCardsOnField.Count == 0) return;

        int match = thisCard.GetComponent<CardStats>().soulUse;
        
        foreach (CardStats card in allCardsOnField)
        {
            if (card == thisCard.GetComponent<CardStats>())
            {
                continue; // skip self
            }

            if (card.soulUse == match)
            {
                card.GetComponent<CardMovement>().ServerDiscard();
            }
        }
        
        AnimateAbilityExecute(thisCard);
    }
}