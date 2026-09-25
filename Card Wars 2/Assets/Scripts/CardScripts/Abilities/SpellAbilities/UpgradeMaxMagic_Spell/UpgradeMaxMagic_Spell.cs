using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeMaxMagic_Spell", menuName = "Abilities/Spell/UpgradeMaxMagic_Spell")]
public class UpgradeMaxMagic_Spell : CastAbilitySO
{
    public int soulIncrease;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        thisCard.Ext_GetOwningPlayerStats().UpdatePlayerMaxSoul_ViaUpgrade(increase: true, amount: soulIncrease, ignoreAnySoulUpgradeBlock: false);
        AnimateAbilityExecute(thisCard);
    }
}
