using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using CardScripts.CardStatss;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DestroyOppFieldCreatures_ThatHaveSameStrength_Creature", 
    menuName = "Abilities/Creature/DestroyOppFieldCreatures_ThatHaveSameStrength_Creature")]
public class DestroyOppFieldCreatures_ThatHaveSameStrength_Creature : ActiveAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get opp field creatures
        PlayerCardTracker oppCardTracker = thisCard.Ext_GetOpponentCardTracker();

        List<CreatureStats> oppsActiveCards = oppCardTracker.Server_GetThisPlayersOnFieldCreatures();

        CreatureStats thisCreature = thisCard.GetComponent<CreatureStats>();
        
        int matchStrength = thisCreature.strength;
        
        foreach (CreatureStats oppStats in oppsActiveCards)
        {
            if (oppStats.strength == matchStrength)
            {
                oppStats.GetComponent<CardMovement>().ServerDiscard();
            }
        }
        
        AnimateAbilityExecute(thisCard);
    }
}