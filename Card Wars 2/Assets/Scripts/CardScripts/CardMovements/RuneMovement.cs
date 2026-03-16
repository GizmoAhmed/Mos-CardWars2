using AbilityEvents;
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

            if (!tile.creatureVisual) return false; // there's no creature on this tile? nope, can't place here

            CreatureStats creature = tile.creatureVisual.GetComponent<CreatureStats>();

            return tile.tileOwner &&
                   tile.creatureVisual != null &&
                   (creature.CanBeRuned); // return true if the tile has a creature on it and creature can be runed
        }

        [Command]
        protected override void CmdPlaceCardOnTile(GameObject tile)
        {
            // base.CmdPlaceCardOnTile(tile);

            Tile tileScript = tile.GetComponent<Tile>();

            GameObject creatureOnTile = tileScript.creatureVisual;

            BindRune(creatureOnTile);

            // register cards passive ability in ability manager as a listener when placed
            RegisterPassiveAbilityToEventManagerInStats();
            
            base.RpcDiscard(); // discard on both clients via base call
        }

        [Server]
        private void BindRune(GameObject creatureOnTile)
        {
            // the creature on this land
            CreatureStats creature = creatureOnTile.GetComponent<CreatureStats>();

            if (creature.currentRune1 == null) // empty
            {
                creature.currentRune1 = GetComponent<RuneBase>(); // goes to RuneChange
            }
            else if (creature.currentRune2 == null && creature.overRuneable)
            {
                creature.currentRune2 = GetComponent<RuneBase>(); // goes to RuneChange
            }
        }
    }
}