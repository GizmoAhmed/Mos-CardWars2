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

        opponent.Server_ChangePlayerSoul(raiseSoul: false, soulReduction);
    }
    
    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats opponent = thisCard.Ext_GetOwningPlayerStats().Ext_GetOpponentPlayerStats();

        opponent.Server_ChangePlayerSoul(raiseSoul: true, soulReduction);
    }
    
    public void OnValidate()
    {
        if (!isExecutableOnPlaced)
        {
            Debug.LogError($"{name} needs to be executable on place");
        }
    }
}
