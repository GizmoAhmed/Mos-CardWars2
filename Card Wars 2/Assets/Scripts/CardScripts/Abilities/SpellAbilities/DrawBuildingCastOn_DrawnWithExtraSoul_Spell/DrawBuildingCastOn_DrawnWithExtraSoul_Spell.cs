using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStats_Folder;
using CardScripts.CardStatss;
using Extensions;
using Tiles;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DrawBuildingCastOn_DrawnWithExtraSoul_Spell", 
    menuName = "Abilities/Spell/DrawBuildingCastOn_DrawnWithExtraSoul_Spell")]
public class DrawBuildingCastOn_DrawnWithExtraSoul_Spell : CastAbilitySO
{
    [Tooltip("The Additional soul cost placed on drawn building")]
    public int extraSoul;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get tile
        MiddleTile middleTile = eventData.target.GetComponent<MiddleTile>();
        
        // see building
        GameObject buildingOnTile = middleTile.logicalBuilding;

        if (buildingOnTile == null) return; // no building to redraw

        GameObject dupeBuilding = RedrawCard(
            buildingOnTile, // redraw building...
            
            //...but make sure it goes to the player that is casting this spell
            drawToThisPlayer: thisCard.Ext_GetOwningPlayerStats() 
            ); 

        // add extra soul
        CardStats stats = dupeBuilding.GetComponent<CardStats>();
        stats.UpdateSyncSoulToPlayer(extraSoul);
    }
}