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

            if (!(tile is MiddleTile midTile)) // has to be middle tile
                return false;
            
            // to place, you have to own this Tile and the Tile can't already have creature on it
            return midTile.tileOwner && midTile.logicalCreature == null;
        }

        [Command] // not needed...todo for now
        protected override void CmdPlaceCardOnTile(GameObject tile)
        {
            base.CmdPlaceCardOnTile(tile); // notice no passive ability registration
        }

        [Server]
        protected override void SetLogicalReferenceOnTile(Tile tile)
        {
            MiddleTile middleTile = tile as MiddleTile;
            
            middleTile.logicalCreature = gameObject;
            // Debug.Log($"Set logical creature ({gameObject.name}) on Tile [{Tile.playerSide}][{Tile.row},{Tile.column}]");
        }

        [Server]
        protected override void ClearLogicalReference_OnTile(Tile tile)
        {
            MiddleTile middleTile = tile as MiddleTile;
            
            if (middleTile.logicalCreature == gameObject)
            {
                middleTile.logicalCreature = null;
            }
        }

        [ClientRpc] // assume valid, so don't worry about ok to place or not
        protected override void RpcPlaceCardOnTile(GameObject tileObj)
        {
            base.RpcPlaceCardOnTile(tileObj);

            MiddleTile midTileScript = tileObj.GetComponent<MiddleTile>();
            MiddleTile visualTile = midTileScript;

            // VISUAL MIRRORING: If this is opponent's card, show on mirrored Tile
            if (!isOwned)
            {
                visualTile = midTileScript.across.GetComponent<MiddleTile>();
            }

            // Visual positioning
            visualTile.creatureVisual = gameObject;
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

        [Server]
        public override void ServerDiscard()
        {
            // if being discarded from the field, returning magic
            if (cardState == CardState.Field) ReturnMagicAndScore();
            
            base.ServerDiscard();
        }

        private void ReturnMagicAndScore()
        {
            thisCardOwnerPlayerStats.currentMagic += cardStats.soulUse; // give back soulUse
            thisCardOwnerPlayerStats.playerTotalScore -= CreatureStats.score; // give back score
        }

        protected override void DetachFromTile()
        {
            ((MiddleTile)currentTileVisual).creatureVisual = null;
            base.DetachFromTile();
        }
    }
}