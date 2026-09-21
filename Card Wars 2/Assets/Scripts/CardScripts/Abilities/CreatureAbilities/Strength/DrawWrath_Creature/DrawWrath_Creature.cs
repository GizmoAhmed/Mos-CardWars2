using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardData;
using CardScripts.CardMovements;
using GameManagement;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(fileName = "DrawWrath_Creature", menuName = "Abilities/Creature/Strength/DrawWrath_Creature")]
public class DrawWrath_Creature : ActiveAbilitySO
{
    public RuneDataSO wrath;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (wrath == null)
        {
            Debug.LogError($"{name} on {thisCard.name} is missing <color=red>wrath</color>, and therefore can't spawn it. Aborting...");
            return;
        }

        // Get owning player
        PlayerStats owningPlayer = thisCard
            .GetComponent<CardMovement>()
            .thisCardOwnerPlayerStats;
        
        MasterDeck masterDeck = FindObjectOfType<MasterDeck>();
        
        masterDeck.CreateThenSpawnCard(wrath.cardID, owningPlayer);
        AnimateAbility(thisCard);
    }
}
