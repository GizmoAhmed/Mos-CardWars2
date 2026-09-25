using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStats_Folder;
using Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseUseOfAllConsumesInHand_OnAbility_Building",
    menuName = "Abilities/Building/IncreaseUseOfAllConsumesInHand_OnAbility_Building")]
public class IncreaseUseOfAllConsumesInHand_OnAbility_Building : PassiveAbilitySO
{
    public int usesIncrease;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get all spells in hand
        List<SpellStats> spells = thisCard.Ext_GetOwningCardTracker().Server_GetThisPlayerInHandSpells();
        
        // increase use on each
        foreach (SpellStats spell in spells)
        {
            spell.uses +=  usesIncrease;
        }

        if (spells.Count > 0)
        {
            AnimateAbilityExecute(thisCard);
        }
    }
}