using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BuffCreature_IfPlacedAsOnlyCreatureOnYourField_Charm",
    menuName = "Abilities/Charm/BuffCreature_IfPlacedAsOnlyCreatureOnYourField_Charm")]
public class BuffCreature_IfPlacedAsOnlyCreatureOnYourField_Charm : PassiveAbilitySO
{
    public int strengthBoost;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats placedCreature = eventData.target.GetComponent<CreatureStats>();

        PlayerStats player = thisCard.Ext_GetOwningPlayerStats();

        bool owned = (placedCreature.gameObject).Ext_IsCardOwnedByThisPlayer(player);

        if (!owned) return; // you didn't place this

        List<CreatureStats> activeCreatures = thisCard.Ext_GetAllActiveCreaturesForThisPlayer();

        if (activeCreatures.Count == 1) // thisCard
        {
            placedCreature.UpdateCreatureDefense(amount: strengthBoost, buff: true, applyMult:true);
            AnimateAbilityExecute(thisCard);
        }
    }
}