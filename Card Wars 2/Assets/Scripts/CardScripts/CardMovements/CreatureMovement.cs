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

            MiddleTile logTile = null;
            
            // if client is validating, check the other side since server saves client side placements on the other side
            if (logicalPlayerSide == 1)
            {
                logTile = midTile.across.GetComponent<MiddleTile>();
            }
            else
            {
                logTile = midTile;
            }

            // the tile has to be on your client side, the bottom row
            // AND the tile has to not have a creature on it
            return midTile.tileOwner && logTile.logicalCreature == null;
        }

        [Command] // not needed...todo for now
        protected override void CmdPlaceCardOnTile(GameObject tile)
        {
            // add bases stats to score and add soul...
            thisCardOwnerPlayerStats.AddPlayerScore(CreatureStats.score);
            thisCardOwnerPlayerStats.UseMagic(CreatureStats.soulUse);
            
            // ... then broadcast, placement abilities need the initial player score to be added first
            base.CmdPlaceCardOnTile(tile); 
        }

        [Server]
        protected override void SetLogicalReferenceOnTile(Tile tile)
        {
            MiddleTile middleTile = tile as MiddleTile;

            // if the client is setting, refer to the tile on the other side for setting logical card
            if (logicalPlayerSide == 1)
            {
                middleTile = middleTile.across.GetComponent<MiddleTile>();
            }

            middleTile.logicalCreature = gameObject;
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
            thisCardsVisualTile = visualTile;
        }

        [Server]
        public override void ServerDiscard()
        {
            // if being discarded from the field, returning magic
            if (cardState == CardState.Field) ReturnMagicAndScore();
            
            // remove all runes
            GetComponentInChildren<RuneSlots>().UnbindAllRunes();
            
            base.ServerDiscard();
        }

        private void ReturnMagicAndScore()
        {
            thisCardOwnerPlayerStats.currentMagic += cardStats.soulUse; // give back soulUse
            thisCardOwnerPlayerStats.playerTotalScore -= CreatureStats.score; // give back score
        }

        protected override void DetachFromTile()
        {
            ((MiddleTile)thisCardsVisualTile).creatureVisual = null;
            base.DetachFromTile();
        }
    }
}