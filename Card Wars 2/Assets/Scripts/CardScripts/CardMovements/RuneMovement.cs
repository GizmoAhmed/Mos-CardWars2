using AbilityEvents;
using CardScripts.CardData;
using CardScripts.CardStats_Folder;
using CardScripts.CardStatss;
using Mirror;
using Tiles;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class RuneMovement : CardMovement
    {
        protected override bool ValidPlacement(Tile tile)
        {
            // if can't get passed global checks, abort
            if (!base.ValidPlacement(tile))
                return false;
            
            if (!(tile is MiddleTile localMidTile)) // has to be middle tile
                return false;
            
            MiddleTile logTile = null;
            
            // if client is validating, check the other side since server saves client side placements on the other side
            logTile = logicalPlayerSide == 1 ? localMidTile.across.GetComponent<MiddleTile>() : localMidTile;

            if (!logTile.logicalCreature) return false; // there's no creature on this middleTile? nope, can't place here

            CreatureStats creature = logTile.logicalCreature.GetComponent<CreatureStats>();

            // get rune slot to see if can be runed
            RuneSlots runeSlot = creature.transform.GetComponentInChildren<RuneSlots>();
            
            return localMidTile.tileOwner &&
                   logTile.logicalCreature != null &&
                   (runeSlot.CanBeRuned); // return true if the middleTile has a creature on it and creature can be runed
        }

        [Command]
        protected override void CmdPlaceCardOnTile(GameObject tile)
        {
            // base.CmdPlaceCardOnTile(middleTile);

            MiddleTile middleTileScript = tile.GetComponent<MiddleTile>();

            MiddleTile logTile = null;
            
            // if client is validating, check the other side since server saves client side placements on the other side
            logTile = logicalPlayerSide == 1 ? middleTileScript.across.GetComponent<MiddleTile>() : middleTileScript;
            
            GameObject creatureOnTile = logTile.logicalCreature;

            PlaceRuneOnCreature(creatureOnTile);
            
            // base.ServerDiscard(); 
        }

        [Server]
        private void PlaceRuneOnCreature(GameObject creatureOnTile)
        {
            // add to RuneSlots
            RuneSlots runeSlot = creatureOnTile.transform.GetComponentInChildren<RuneSlots>();
            
            runeSlot.BindRune(gameObject);
        }
    }
}