using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ExecuteCreatureAbility_OnCast_Spell", 
    menuName = "Abilities/Spell/ExecuteCreatureAbility_OnCast_Spell")]
public class ExecuteCreatureAbility_OnCast_Spell : CastAbilitySO
{
    // TODO: Add your ability parameters here
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get creature
        CreatureStats creature = eventData.Ext_GetCreatureStats_FromSpellCastEventData();

        if (creature == null) return;

        // floop creature
        creature.ActivateCreatureAbility();
    }
}