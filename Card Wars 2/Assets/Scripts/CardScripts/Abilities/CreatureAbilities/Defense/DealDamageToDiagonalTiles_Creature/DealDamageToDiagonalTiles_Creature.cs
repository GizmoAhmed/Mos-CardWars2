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
    fileName = "DealDamageToDiagonalTiles_Creature",
    menuName = "Abilities/Creature/DealDamageToDiagonalTiles_Creature")]
public class DealDamageToDiagonalTiles_Creature : ActiveAbilitySO
{
    public int damage;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        Debug.Log($"Executing on {this} on {thisCard.name}");

        List<MiddleTile> diagTiles = thisCard.Ext_GetTilesDiagonalToThisCard();

        foreach (MiddleTile tile in diagTiles)
        {
            tile.Ext_DamageTile(damage, source: thisCard.transform); // deal damage to tile
        }
        
        // AnimateAbilityExecute(thisCard);
    }
}