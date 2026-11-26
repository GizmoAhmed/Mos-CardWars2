using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "StrPerTileOnFieldWithoutCreature_Creature", 
    menuName = "Abilities/Creature/Strength/StrPerTileOnFieldWithoutCreature_Creature")]
public class StrPerTileOnFieldWithoutCreature_Creature : ActiveAbilitySO
{
    public int strBuffPerEmptyTile;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        List<MiddleTile> allTiles = thisCard.Ext_GetAllMiddleTiles();

        int buff = 0;

        foreach (MiddleTile tile in allTiles)
        {
            if (!tile.HasCreature) // if has no creature
            {
                buff += strBuffPerEmptyTile;
            }
        }
        
        CreatureStats stats = thisCard.GetComponent<CreatureStats>();
        
        stats.UpdateCreatureStrength(amount: buff, buff: true, applyMult: true, source: null);
    }
}