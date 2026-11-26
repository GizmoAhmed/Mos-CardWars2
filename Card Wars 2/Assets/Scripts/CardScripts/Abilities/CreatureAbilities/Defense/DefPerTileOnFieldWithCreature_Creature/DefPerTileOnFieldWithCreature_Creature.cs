using System.Linq;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using Tiles;

[CreateAssetMenu(
    fileName = "DefPerTileOnFieldWithCreature_Creature",
    menuName = "Abilities/Creature/Defense/DefPerTileOnFieldWithCreature_Creature")]
public class DefPerTileOnFieldWithCreature_Creature : ActiveAbilitySO
{
    public int defBuffPerOccupiedTile;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        int occupiedTiles = thisCard.Ext_GetAllMiddleTiles().Count(tile => tile.HasCreature);
        if (occupiedTiles == 0) return;

        CreatureStats stats = thisCard.GetComponent<CreatureStats>();
        stats.UpdateCreatureDefense(occupiedTiles * defBuffPerOccupiedTile, buff: true);
    }
}