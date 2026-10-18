using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "ShareBuffsAndNerfsOfAnAdjacentCreature_Building", 
    menuName = "Abilities/Building/ShareBuffsAndNerfsOfAnAdjacentCreature_Building")]
public class ShareBuffsAndNerfsOfAnAdjacentCreature_Building : PassiveAbilitySO
{
    public AdjacentSide side;

    public enum AdjacentSide
    {
        Left,
        Right
    }
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureSharingThisTile = thisCard.Ext_GetCreatureStats_FromSharedBuildingsTile();

        if (creatureSharingThisTile == null) return; // no point
        
        CreatureStats creatureBuffed = eventData.target.GetComponent<CreatureStats>();

        if (!creatureBuffed.gameObject.Ext_IsSameOwner(thisCard)) return;  // if not owned, return
        
        string leftOrRight = side == AdjacentSide.Left ? "left" : "right";
        
        // the creature adjacent to the tile of this building
        GameObject adjCreature = GetAnAdjacentCreature(thisCard, leftOrRight);
        if (adjCreature == null) return;

        // is the creature that was just buffed the creature adjacent to this buildings tile
        if (adjCreature == creatureBuffed.gameObject) // the one next to this one
        {
            int statAmount = eventData.Value;
            
            switch (eventData.eventType)
            {
                case AbilityEventType.AnyCreatureStrengthBuff:
                    creatureSharingThisTile.UpdateCreatureStrength(amount: statAmount, buff: true);
                    break;
                case AbilityEventType.AnyCreatureDefenseBuff:
                    creatureSharingThisTile.UpdateCreatureDefense(amount: statAmount, buff: true);
                    break;
                case AbilityEventType.AnyCreatureStrengthNerf:
                    creatureSharingThisTile.UpdateCreatureStrength(amount: statAmount, buff: false);
                    break;
                case AbilityEventType.AnyCreatureDefenseNerf:
                    creatureSharingThisTile.UpdateCreatureDefense(amount: statAmount, buff: false);
                    break;
                default:
                    Debug.LogError($"{name} on {thisCard.name} Unrecognizable Ability EventType passed: {eventData.eventType}");
                    break;
            }
            
        }
    }
}