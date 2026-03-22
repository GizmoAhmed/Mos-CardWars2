using CardScripts.CardData;
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
            
            if (!(tile is MiddleTile midTile)) // has to be middle tile
                return false;

            // return true if one of your own lands, the close ones
            return midTile.tileOwner && midTile.logicalBuilding == null;
        }

        [Command]
        protected override void CmdPlaceCardOnTile(GameObject tile)
        {
            base.CmdPlaceCardOnTile(tile); 
            
            PassiveListenerCard listener = GetComponent<PassiveListenerCard>();
            
            if (listener == null)
            {
                Debug.LogError($"No listener found on {gameObject.name}");
                return;
            }

            // register cards passive ability in ability manager as a listener when placed
            listener.RegisterPassiveAbility();
        }
        
        [Server]
        protected override void SetLogicalReferenceOnTile(Tile tile)
        {
            MiddleTile middleTile = tile as MiddleTile;

            
            middleTile.logicalBuilding = gameObject;
            // Debug.Log($"Set logical building ({gameObject.name}) on Tile [{Tile.playerSide}][{Tile.row},{Tile.column}]");
        }
    
        [Server]
        protected override void ClearLogicalReferenceOnTile(Tile tile)
        {
            MiddleTile middleTile = tile as MiddleTile;
            
            if (middleTile.logicalBuilding == gameObject)
            {
                middleTile.logicalBuilding = null;
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
            visualTile.buildingVisual = gameObject;
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
            ((MiddleTile)currentTileVisual).buildingVisual = null;
            base.DetachFromTile();
        }
    }
}