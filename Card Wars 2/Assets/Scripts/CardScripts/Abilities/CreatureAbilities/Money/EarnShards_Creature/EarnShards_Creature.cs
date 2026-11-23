using AbilityEvents;
using CardScripts.Abilities;
using Extensions;
using UnityEngine;
using PlayerStuff;

[CreateAssetMenu(
    fileName = "EarnShards_Creature", 
    menuName = "Abilities/Creature/EarnShards_Creature")]
public class EarnShards_Creature : ActiveAbilitySO
{
    public int shards;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats player = thisCard.Ext_GetOwningPlayerStats();
        
        player.shards += shards;
        
        AnimateAbilityExecute(thisCard);
    }
}