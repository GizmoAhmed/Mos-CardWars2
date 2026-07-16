using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStatss;
using Extensions;
using Tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicWeaken_OnCreature_Spell", menuName = "Abilities/Spell/BasicWeaken_OnCreature_Spell")]
public class BasicWeaken_OnCreature_Spell : CastAbilitySO
{
    public int weakenAmount;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureStats = eventData.Ext_GetCreatureStats_FromSpellCastEventData();

        if (creatureStats == null)
            return; // error message inside above function
        
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
