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
        
        // todo notice ↓

        // when this is called consecutively (ie pride), it stacks the calls
        // so if this is +2, pride will right +4, giving the illusion that this ability only went off once
        player.shards += shards;
        
        AnimateAbilityExecute(thisCard);
    }
}