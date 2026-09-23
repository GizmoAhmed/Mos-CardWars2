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

    [Header("Chance to Destroy Each Turn")]
    [Range(1, 100)] public int destroyChance;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (RollChance(destroyChance)) // hit, destroy this building
        {
            BuildingMovement buildingMovement = thisCard.GetComponent<BuildingMovement>();
            
            buildingMovement.ServerDiscard();
        }
        else // miss, buff
        {
            CreatureStats creatureStats = thisCard.Ext_GetCreatureStats_FromSharedBuildingsTile();

            if (creatureStats != null) // null error check happens in above extension method
            {
                creatureStats.UpdateCreatureStrength(strengthBoost, buff: true);
                creatureStats.UpdateCreatureDefense(defenseBoost, buff: true);
                AnimateAbilityExecute(thisCard);
            }
        }
    }
}
