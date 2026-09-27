using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FortifyCreature_ForEachXShardPlayerHas_Building",
    menuName = "Abilities/Building/FortifyCreature_ForEachXShardPlayerHas_Building")]
public class FortifyCreature_ForEachXShardPlayerHas_Building : PassiveAbilitySO
{
    [Tooltip("How much fortify given...")] public int fortify; // how much you fortify...

    [Tooltip("...For this many shard")] public int perShard; // For each shard

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureOnTile = thisCard.Ext_GetCreatureStats_FromSharedBuildingsTile();
        if (creatureOnTile == null) return;

        // get player money
        PlayerStats player = thisCard.Ext_GetOwningPlayerStats();
        int shards = player.shards;
        int rate = shards / perShard;  // floors
        
        int buffAmount = rate * fortify;
        
        creatureOnTile.UpdateCreatureDefense(amount: buffAmount, buff: true);
        AnimateAbilityExecute(thisCard);
    }
}