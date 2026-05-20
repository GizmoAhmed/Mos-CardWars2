using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using GameManagement;
using Tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageAllOppCreatures_Spell", menuName = "Abilities/Spell/DamageAllOppCreatures_Spell")]
public class DamageAllOppCreatures_Spell : CastAbilitySO
{
    public int damagetoAllAmount;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        Debug.Log($"{thisCard.name} damages all opponents for {damagetoAllAmount}");
        
        SpellMovement move = thisCard.GetComponent<SpellMovement>();

        int otherSide = (move.logicalPlayerSide == 0) ? 1 : 0;
        
        List<MiddleTile> oppTiles = TileManager.Instance.GetTilesForPlayer(otherSide);

        // foreach opp tile, deal damage to creature on said tile, if there is a creature
        foreach (MiddleTile tile in oppTiles)
        {
            if (tile.logicalCreature != null)
            {
                CreatureStats stats = tile.logicalCreature.GetComponent<CreatureStats>();
                
                stats.ChangeCreatureDefense(damagetoAllAmount, false);
            }
        }
    }
    
    public void OnValidate()
    {
        if (castRequirementType != CastRequirementType.AnyTile)
        {
            Debug.LogError($"{name} should have cast type {CastRequirementType.AnyTile}");
        }
        
        if (castSide != CastSide.Theirs)
        {
            Debug.LogError($"{name} should have cast side of {CastSide.Theirs}");
        }
    }
}
