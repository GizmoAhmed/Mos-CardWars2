using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using UnityEngine;
using Extensions;
using PlayerStuff;

[CreateAssetMenu(fileName = "SpendAllShards_BuffAllYourCreatures_Creature",
    menuName = "Abilities/Creature/Money/SpendAllShards_BuffAllYourCreatures_Creature")]
public class SpendAllShards_BuffAllYourCreatures_Creature : ActiveAbilitySO
{
    public int strengthPerShard;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (strengthPerShard <= 0)
        {
            Debug.LogWarning(
                $"{name} on {thisCard.name} doesn't have buff rate set ({strengthPerShard}), aborting floop");
            return;
        }

        CreatureStats stats = thisCard.GetComponent<CreatureStats>();
        
        if (stats.abilityCost > 0)
        {
            Debug.LogWarning($"{name} on {thisCard.name} has an ability cost, which is weird given what this ability does. Take a look");
        }

        PlayerStats player = thisCard.Ext_GetOwningPlayerStats();

        int currentShards = player.shards;

        if (currentShards <= 0) return; // actually need shards for this ability work

        int buffAmount = currentShards * strengthPerShard;
        
        // makes sense to lose all money first,
        // since buffing creatures could just give the incorrect amount of money if money gained off buff

        player.shards -= currentShards;
        
        List<CreatureStats> creatures = thisCard.Ext_GetAllActiveCreaturesForThisPlayer();

        foreach (CreatureStats creature in creatures) // buff each creature
        {
            creature.UpdateCreatureStrength(buffAmount, buff: true);
        }
        AnimateAbilityExecute(thisCard);
    }
}