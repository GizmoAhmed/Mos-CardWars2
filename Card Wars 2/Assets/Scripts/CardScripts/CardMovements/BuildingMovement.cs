using Mirror;
using PlayerStuff;
using Tiles;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class BuildingMovement : CardMovement
    {
        protected override bool ValidPlacement(Tile tile)
        {
            // if can't get passed global checks, abort
            if (!base.ValidPlacement(tile))
                return false;

            // return true if one of your own lands, the close ones
            return tile.tileOwner && tile.building == null;
        }

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
            visualTile.building = gameObject;
            transform.SetParent(visualTile.transform, false);
            transform.localPosition = new Vector3(-40,-35,0);
            transform.SetAsFirstSibling();

            // Update visual reference
            currentTileVisual = visualTile;

            // Player-specific logic
            if (thisCardOwnerPlayerStats != null)
            {
                thisCardOwnerPlayerStats.UseMagic(cardStats.soulUse);
            }

            Debug.Log($"Client: Visual tile = {visualTile.gameObject.name}, " +
                      $"Logical position = Row {logicalRow}, Col {logicalColumn}");
        }

        protected override void Discard()
        {
            if (cardState == CardState.Field)
            {
                // change player sync vars requires server call
                ReturnMagic();
            }

            base.Discard();
        }

        [Command]
        private void ReturnMagic()
        {
            thisCardOwnerPlayerStats.currentMagic += cardStats.soulUse; // give back soulUse
        }

        protected override void DetachFromTile()
        {
            currentTileVisual.building = null;
            base.DetachFromTile();
        }
    }
}