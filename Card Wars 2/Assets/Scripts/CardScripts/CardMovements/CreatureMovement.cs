using CardScripts.CardStatss;
using Mirror;
using PlayerStuff;
using Tiles;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class CreatureMovement : CardMovement // these are creatures and buildings
    {
        private CreatureStats CreatureStats => cardStats as CreatureStats;

        protected override bool ValidPlacement(Tile tile)
        {
            // if can't get passed global checks, abort
            if (!base.ValidPlacement(tile))
                return false;
            
            return tile.tileOwner && tile.creature == null && !tile.IsOccupied;
        }

        /*[Command] // not needed...todo for now
        protected override void CmdPlaceCardOnTile(GameObject tile)
        {
            base.CmdPlaceCardOnTile(tile); // notice no passive ability registration
        }*/

        [ClientRpc] // assume valid, so don't worry about ok to place or not
        protected override void RpcPlaceCardOnTile(GameObject tileObj)
        {
            base.RpcPlaceCardOnTile(tileObj);

            Tile tileScript = tileObj.GetComponent<Tile>();
            Tile visualTile = tileScript;

            // VISUAL MIRRORING: If this is opponent's card, show on mirrored tile
            if (!isOwned)
            {
                visualTile = tileScript.across.GetComponent<Tile>();
            }

            // Visual positioning
            visualTile.creature = gameObject;
            transform.SetParent(visualTile.transform, false);
            transform.localPosition = Vector3.zero;
            transform.SetAsFirstSibling();
        
            // Update visual reference
            currentTileVisual = visualTile;
        
            if (thisCardOwnerPlayerStats != null)
            {
                thisCardOwnerPlayerStats.AddPlayerScore(CreatureStats.score);
                thisCardOwnerPlayerStats.UseMagic(CreatureStats.soulUse);
            }
        }

        protected override void Discard()
        {
            if (cardState == CardState.Field)
            {
                // change player sync vars requires server call, 
                ReturnMagicAndScore(); 
            }

            base.Discard();
        }

        [Command]
        private void ReturnMagicAndScore()
        {
            thisCardOwnerPlayerStats.currentMagic += cardStats.soulUse; // give back soulUse
            thisCardOwnerPlayerStats.playerTotalScore -= CreatureStats.score; // give back score
        }

        protected override void DetachFromTile()
        {
            currentTileVisual.creature = null;
            base.DetachFromTile();
        }
    }
}