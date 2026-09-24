using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using PlayerStuff;
using UnityEngine;


[CreateAssetMenu(fileName = "Strengthen_ForEachUnusedSoulOnOpp_Creature", menuName = "Abilities/Creature/Soul/Strengthen_ForEachUnusedSoulOnOpp_Creature")]
public class Strengthen_ForEachUnusedSoulOnOpp_Creature : ActiveAbilitySO
{
    public float strPerCurrentSoul;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // read op current soul
        PlayerStats opp = thisCard.Ext_GetOwningPlayerStats().Ext_GetOpponentPlayerStats();
        
        int oppCurrentSoul = opp.currentSoul;
        int str = (int)(oppCurrentSoul * strPerCurrentSoul); // floor
        
        // buff at rate
        thisCard.GetComponent<CreatureStats>().UpdateCreatureStrength(amount: str, buff: true);
        AnimateAbilityExecute(thisCard);
    }
}
