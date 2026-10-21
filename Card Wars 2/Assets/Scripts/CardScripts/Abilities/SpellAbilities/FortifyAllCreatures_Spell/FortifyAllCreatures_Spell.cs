using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "FortifyAllCreatures_Spell", 
    menuName = "Abilities/Spell/FortifyAllCreatures_Spell")]
public class FortifyAllCreatures_Spell : CastAbilitySO
{
    public int fortify;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        List<CreatureStats> allCreatures = thisCard.Ext_GetAllActiveCreaturesForThisPlayer();

        foreach (CreatureStats creature in allCreatures)
        {
            creature.UpdateCreatureDefense(amount: fortify, buff: true);
        }
    }
}