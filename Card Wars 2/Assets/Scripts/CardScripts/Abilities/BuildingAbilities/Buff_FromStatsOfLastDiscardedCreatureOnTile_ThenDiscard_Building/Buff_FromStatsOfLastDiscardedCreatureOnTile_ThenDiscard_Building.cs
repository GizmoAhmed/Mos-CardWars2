using AbilityEvents;
using CardScripts;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using CardScripts.CardMovements;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Buff_FromStatsOfLastDiscardedCreatureOnTile_ThenDiscard_Building", 
    menuName = "Abilities/Building/Buff_FromStatsOfLastDiscardedCreatureOnTile_ThenDiscard_Building")]
public class Buff_FromStatsOfLastDiscardedCreatureOnTile_ThenDiscard_Building : PassiveAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CardRuntimeData data = thisCard.GetComponent<CardRuntimeData>();
        
        if (eventData.eventType == AbilityEventType.CardDiscardedFromTile)
        {
            // a creature that was nerfed was discarded because of said nerf, ignore
            if (data.customInts.Count != 0) return;

            CreatureStats discardedCreature = eventData.target.GetComponent<CreatureStats>();
            
            int strength = discardedCreature.strength;
            int defense = discardedCreature.defense;
            
            // save
            data.customInts["str"] =  strength;
            data.customInts["def"] =  defense;
        }
        else if (eventData.eventType == AbilityEventType.CardPlacedOnTile)
        {
            CreatureStats placedCreature = eventData.target.GetComponent<CreatureStats>();

            if (data.customInts.Count == 0) return; // no creature's stats were saved
            
            // save these here, since discarding below deletes them
            int savedStrength = data.customInts["str"];
            int savedDefense = data.customInts["def"];
            
            // if got here means stats being transferred, have to discard this first since,
            // because doing after causes a loop that I couldn't figure out
            thisCard.GetComponent<BuildingMovement>().ServerDiscard();

            switch (savedStrength)
            {
                // positive, buff
                case > 0:
                    placedCreature.UpdateCreatureStrength(amount: savedStrength, buff: true);
                    break;
                // negative
                case < 0:
                    placedCreature.UpdateCreatureStrength(amount: savedStrength * -1, buff: false);
                    break;
            }
            
            switch (savedDefense)
            {
                // positive, buff
                case > 0:
                    placedCreature.UpdateCreatureDefense(amount: savedDefense, buff: true);
                    break;
                // negative
                case < 0:
                    // make positive then nerf
                    placedCreature.UpdateCreatureDefense(amount: savedDefense * -1, buff: false);
                    break;
            }
        }
    }
}