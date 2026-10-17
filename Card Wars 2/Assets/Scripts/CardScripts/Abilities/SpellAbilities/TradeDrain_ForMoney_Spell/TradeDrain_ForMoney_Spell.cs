using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "TradeDrain_ForMoney_Spell",
    menuName = "Abilities/Spell/TradeDrain_ForMoney_Spell")]
public class TradeDrain_ForMoney_Spell : CastAbilitySO
{
    public int drainLoss;
    public int shardGain;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats player = thisCard.Ext_GetOwningPlayerStats();

        player.drain -= drainLoss;
        player.shards += shardGain;
    }
}