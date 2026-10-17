using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;

[CreateAssetMenu(
    fileName = "BuffCreatureOnTile_WhenYouHitOnAnyLuck_Building", 
    menuName = "Abilities/Building/BuffCreatureOnTile_WhenYouHitOnAnyLuck_Building")]
public class BuffCreatureOnTile_WhenYouHitOnAnyLuck_Building : PassiveAbilitySO
{
    public int strength;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        GameObject luckyCard = eventData.target.gameObject;

        if (!luckyCard.Ext_IsSameOwner(thisCard)) return; // if you own the luck card

        CreatureStats creatureOnTile = thisCard.Ext_GetCreatureStats_FromSharedBuildingsTile();
        
        if (creatureOnTile == null) return;
        
        creatureOnTile.UpdateCreatureStrength(amount: strength, buff: true, applyMult:true);
    }
}