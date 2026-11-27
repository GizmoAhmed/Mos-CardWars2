using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "ReduceSoulSelf_Creature", 
    menuName = "Abilities/Creature/Soul/ReduceSoulSelf_Creature")]
public class ReduceSoulSelf_Creature : ActiveAbilitySO
{
    public int soulReduction;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creature = thisCard.GetComponent<CreatureStats>();
        
        creature.UpdateSyncSoulToPlayer(diff: -soulReduction); // it adds this much, so send it as negative
        
        // AnimateAbilityExecute(thisCard);
    }
}