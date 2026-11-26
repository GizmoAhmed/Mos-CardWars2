using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "StrengthenThisForEachMaxSoul_Creature", 
    menuName = "Abilities/Creature/Soul/StrengthenThisForEachMaxSoul_Creature")]
public class StrengthenThisForEachMaxSoul_Creature : ActiveAbilitySO
{
    public int strength;
    public int forEverySoul;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats playerStats = thisCard.Ext_GetOwningPlayerStats();
        
        int maxSoul  = playerStats.maxSoul; // get player max soul
        
        int each = maxSoul / forEverySoul; // divide to get soul mult
        
        int buff = each * strength; // str for each

        CreatureStats creature = thisCard.GetComponent<CreatureStats>();
        
        creature.UpdateCreatureStrength(amount: buff, buff: true, applyMult: true); // buff
    }
}