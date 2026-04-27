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

            MiddleTile logTile = GetServerTileForClient(midTile) as MiddleTile;

            // return true if one of your own lands, the close ones
            return midTile.clientTileOwner && logTile.logicalBuilding == null;
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
            
            // if the client is setting, refer to the tile on the other side for setting logical card
            if (logicalPlayerSide == 1)
            {
                middleTile = middleTile.across.GetComponent<MiddleTile>();
            }
            
            middleTile.logicalBuilding = gameObject;
        }
    
        [Server]
        protected override void ClearLogicalReference_OnTile(Tile tile)
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
            thisCardsVisualTile = visualTile;

            // Player-specific logic
            if (thisCardOwnerPlayerStats != null)
            {
                thisCardOwnerPlayerStats.UseMagic(cardStats.soulUse);
            }
        }

        [Server]
        public override void ServerDiscard()
        {
            // if being discarded from the field, returning magic
            if (cardState == CardState.Field) ReturnMagic();
            
            base.ServerDiscard();
        }

        [Command]
        private void ReturnMagic()
        {
            thisCardOwnerPlayerStats.currentMagic += cardStats.soulUse; // give back soulUse
        }

        protected override void DetachFromTile()
        {
            ((MiddleTile)thisCardsVisualTile).buildingVisual = null;
            base.DetachFromTile();
        }
    }
}