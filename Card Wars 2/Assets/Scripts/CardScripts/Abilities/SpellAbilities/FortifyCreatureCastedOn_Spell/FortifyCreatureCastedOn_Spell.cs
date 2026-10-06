using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FortifyCreatureCastedOn_Spell", 
    menuName = "Abilities/Spell/FortifyCreatureCastedOn_Spell")]
public class FortifyCreatureCastedOn_Spell : CastAbilitySO
{
    public int fortify;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureOnTile = eventData.Ext_GetCreatureStats_FromSpellCastEventData();

        if (creatureOnTile == null) return; // shouldn't tho
        
        creatureOnTile.UpdateCreatureDefense(amount: fortify, buff: true, applyMult: true);
    }
}