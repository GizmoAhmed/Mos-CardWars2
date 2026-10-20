using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;
using CardScripts.CardMovements;

[CreateAssetMenu(
    fileName = "DiscardACreature_DrawStrongerVersion_Spell", 
    menuName = "Abilities/Spell/DiscardACreature_DrawStrongerVersion_Spell")]
public class DiscardACreature_DrawStrongerVersion_Spell : CastAbilitySO
{
    public int abilityCostReduction;
    public int soulReduction;
    public int defIncrease;
    public int strIncrease;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
     {
         MiddleTile middleTile = eventData.target.GetComponent<MiddleTile>();
         
         GameObject creatureOnTile = middleTile.logicalCreature;
         
         // draw creature
         GameObject dupeCreature = RedrawCard(
             creatureOnTile, // redraw building...
             
             //...but make sure it goes to the player that is casting this spell
             drawToThisPlayer: thisCard.Ext_GetOwningPlayerStats() 
         ); 
         
         // buff drawn creature
         CreatureStats dupeStats = dupeCreature.GetComponent<CreatureStats>();
         
         dupeStats.UpdateCreatureDefense(amount: defIncrease, buff:true, applyMult: false);
         dupeStats.UpdateCreatureStrength(amount: strIncrease, buff:true, applyMult: false);
         dupeStats.UpdateSyncSoulToPlayer(-soulReduction);
         dupeStats.ChangeAbilityCost(abilityCostReduction, increase:false);
         
         // destroy creature on tile
         creatureOnTile.GetComponent<CreatureMovement>().ServerDiscard();
     }
}