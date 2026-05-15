using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStatss;
using Tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicWeaken_OnCreature_Spell", menuName = "Abilities/Spell/BasicWeaken_OnCreature_Spell")]
public class BasicWeaken_OnCreature_Spell : CastAbilitySO
{
    public int weakenAmount;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        MiddleTile middleTile = eventData.CardToBeAffected.GetComponent<MiddleTile>();
        GameObject creatureOnTile = middleTile.logicalCreature;
            
        if (creatureOnTile == null)
        {
            Debug.LogError($"{name} could not find creature on middleTile {middleTile.gameObject.name}");
            return;
        }
            
        CreatureStats creatureStats = creatureOnTile.GetComponent<CreatureStats>();
            
        // weaken creature
        creatureStats.ChangeCreatureStrength(weakenAmount, buff: false);
    }

    public void OnValidate()
    {
        if (castRequirementType != CastRequirementType.OnCreature)
        {
            Debug.LogError($"{name} should have cast type {CastRequirementType.OnCreature}");
        } 
    }
}
