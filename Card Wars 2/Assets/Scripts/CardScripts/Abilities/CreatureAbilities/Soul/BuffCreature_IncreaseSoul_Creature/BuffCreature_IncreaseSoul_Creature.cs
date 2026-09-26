using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffCreature_IncreaseSoul_Creature", menuName = "Abilities/Creature/Soul/BuffCreature_IncreaseSoul_Creature")]
public class BuffCreature_IncreaseSoul_Creature : ActiveAbilitySO
{
    public int fortify;
    public int strengthen;

    public int soulIncrease;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats stats = thisCard.GetComponent<CreatureStats>();
        stats.UpdateCreatureStrength(amount: strengthen, buff:true);
        stats.UpdateCreatureDefense(amount: fortify, buff:true);
        stats.UpdateSyncSoulToPlayer(soulIncrease);
        AnimateAbilityExecute(thisCard);
    }
}
