using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeMagic_OnCreatureBurnedOnTile_IfCertainScore_Building", menuName = "Abilities/Building/UpgradeMagic_OnCreatureBurnedOnTile_IfCertainScore_Building")]
public class UpgradeMagic_OnCreatureBurnedOnTile_IfCertainScore_Building : PassiveAbilitySO
{
    [Tooltip("Burned creature needs at least this score to upgrade magic")]
    public int scoreNeeded;

    [Tooltip("How much soul and max soul you want to upgrade by")]
    public int soulUpgradeAmount;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // because tile based, I don't have to check for tile, if it's on the field, or owned by the player...
        //... all of that stuff is implied
        
        GameObject burnedCreature = eventData.target;
        
        CreatureStats creatureStats = burnedCreature.GetComponent<CreatureStats>();

        if (creatureStats.score >= scoreNeeded) // pass
        {
            PlayerStats player = thisCard.Ext_GetOwningPlayerStats();

            player.UpdatePlayerMaxSoul_ViaUpgrade(increase: true, amount: soulUpgradeAmount);
        }
        else
        {
            // this execute does nothing and the creature just burns
        }
    }
}
