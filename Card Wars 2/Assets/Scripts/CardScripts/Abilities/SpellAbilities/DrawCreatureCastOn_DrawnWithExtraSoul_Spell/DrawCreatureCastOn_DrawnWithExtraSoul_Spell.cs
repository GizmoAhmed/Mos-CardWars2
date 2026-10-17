using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStats_Folder;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "DrawCreatureCastOn_DrawnWithExtraSoul_Spell", 
    menuName = "Abilities/Spell/DrawCreatureCastOn_DrawnWithExtraSoul_Spell")]
public class DrawCreatureCastOn_DrawnWithExtraSoul_Spell : CastAbilitySO
{
    [Tooltip("The Additional soul cost placed on drawn creature")]
    public int extraSoul;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        MiddleTile middleTile = eventData.target.GetComponent<MiddleTile>();

        GameObject creatureOnTile = middleTile.logicalCreature;
        
        GameObject dupeCreature = RedrawCard(
            creatureOnTile, // redraw building...
            
            //...but make sure it goes to the player that is casting this spell
            drawToThisPlayer: thisCard.Ext_GetOwningPlayerStats() 
        ); 
        
        CardStats stats = dupeCreature.GetComponent<CardStats>();
        stats.UpdateSyncSoulToPlayer(extraSoul);
    }
}