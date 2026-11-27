using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Extensions;
using Tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffStat_EachTurn_WithChanceToDestroySelf_Building", menuName = "Abilities/Building/BuffStat_EachTurn_WithChanceToDestroySelf_Building")]
public class BuffStat_EachTurn_WithChanceToDestroySelf_Building : PassiveAbilitySO
{
    public int strengthBoost;
    public int defenseBoost;

    [Header("Chance to buff Each Turn")]
    [Range(1, 100)] public int buffChance;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (RollChance(buffChance, thisCard)) // hit, buff
        {
            CreatureStats creatureStats = thisCard.Ext_GetCreatureStats_FromSharedBuildingsTile();

            if (creatureStats != null) // null error check happens in above extension method
            {
                creatureStats.UpdateCreatureStrength(strengthBoost, buff: true, applyMult: true, source: thisCard.transform);
                creatureStats.UpdateCreatureDefense(defenseBoost, buff: true, applyMult: true, source: thisCard.transform);
                // AnimateAbilityExecute(thisCard);
            }
        }
        else // miss, discard this building
        {
            BuildingMovement buildingMovement = thisCard.GetComponent<BuildingMovement>();
            
            buildingMovement.ServerDiscard();
        }
    }
}
