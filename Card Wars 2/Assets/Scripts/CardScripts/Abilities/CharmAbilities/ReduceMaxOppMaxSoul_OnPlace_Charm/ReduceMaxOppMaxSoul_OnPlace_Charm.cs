using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(fileName = "ReduceMaxOppMaxSoul_OnPlace_Charm", menuName = "Abilities/Charm/ReduceMaxOppMaxSoul_OnPlace_Charm")]
public class ReduceMaxOppMaxSoul_OnPlace_Charm : PassiveAbilitySO
{
    public int soulReduction;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats opponent = thisCard.Ext_GetOwningPlayerStats().Ext_GetOpponentPlayerStats();

        opponent.UpdatePlayerMaxSoul_ViaUpgrade(increase: false, soulReduction);
        AnimateAbilityExecute(thisCard);
    }
    
    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats opponent = thisCard.Ext_GetOwningPlayerStats().Ext_GetOpponentPlayerStats();

        // this undo shouldn't be blocked by anything
        opponent.UpdatePlayerMaxSoul_ViaUpgrade(increase: true, soulReduction, ignoreAnySoulUpgradeBlock: true);
    }
    
    public void OnValidate()
    {
        if (!isExecutableOnPlaced)
        {
            Debug.LogError($"{name} needs to be executable on place");
        }
    }
}
