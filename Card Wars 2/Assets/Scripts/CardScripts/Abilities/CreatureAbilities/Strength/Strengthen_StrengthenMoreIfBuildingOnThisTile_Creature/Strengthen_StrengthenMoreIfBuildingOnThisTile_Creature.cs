using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using Tiles;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Strengthen_StrengthenMoreIfBuildingOnThisTile_Creature", 
    menuName = "Abilities/Creature/Strengthen_StrengthenMoreIfBuildingOnThisTile_Creature")]
public class Strengthen_StrengthenMoreIfBuildingOnThisTile_Creature : ActiveAbilitySO
{
    public int defaultStrength;
    public int bonusStrength;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        MiddleTile tile = thisCard.Ext_GetTile() as MiddleTile;
        
        CreatureStats stats = thisCard.GetComponent<CreatureStats>();

        if (tile.logicalBuilding) // if building present
        {
            stats.UpdateCreatureStrength(amount: bonusStrength, buff: true);
        }
        else
        {
            stats.UpdateCreatureStrength(defaultStrength, buff: true);
        }
        
        AnimateAbilityExecute(thisCard);
    }
}