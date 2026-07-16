using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffOrDestroy_OnChance_Spell", menuName = "Abilities/Spell/BuffOrDestroy_OnChance_Spell")]
public class BuffOrDestroy_OnChance_Spell : CastAbilitySO
{
    [Header("Chance to Hit")]
    [Range(1, 100)] public int chance;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureOnTile = eventData.Ext_GetCreatureStats_FromSpellCastEventData();
            
        if (creatureOnTile == null)
            return; // error message inside above function
        
        if (RollChance(chance)) // hit
        {
            Debug.Log($"{name} hit! Buffing {creatureOnTile.name}");
            
            // chance between attack or defense
            if (RollChance(50))
            {
                int defense = creatureOnTile.defense;
                creatureOnTile.ChangeCreatureDefense(defense, buff: true);
            }
            else
            {
                int strength = creatureOnTile.strength;
                creatureOnTile.ChangeCreatureStrength(strength, buff: true);
            }
        }
        else // miss
        {
            Debug.LogWarning($"{name} missed! Discarding {creatureOnTile.name}");
            creatureOnTile.GetComponent<CreatureMovement>().ServerDiscard();
        }
    }
}
