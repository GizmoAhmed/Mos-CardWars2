using System;
using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using GameManagement;
using Tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "DestroyAllCardsInLane_Spell", menuName = "Abilities/Spell/DestroyAllCardsInLane_Spell")]
public class DestroyAllCardsInLane_Spell : CastAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        Tile thisTile = eventData.target.GetComponent<Tile>();

        if (thisTile == null) return;

        Tile across = TileManager.Instance.GetAcrossTile
        (
            thisTile.row,
            thisTile.column,
            thisTile.serverPlayerSide
        );

        // destroy all cards on both sides
        across.DestroyAllCardsOnTile();
        thisTile.DestroyAllCardsOnTile();
    }
}