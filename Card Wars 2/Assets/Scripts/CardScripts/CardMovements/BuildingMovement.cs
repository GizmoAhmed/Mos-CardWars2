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
            
            if (!isOwned)
            {
                tileObj = tileScript.across;
                tileScript = tileObj.GetComponent<Tile>();
            }

            tileScript.building = gameObject; // set tiles building as this

            transform.SetParent(tileObj.transform, false); // set card as child of tile
            transform.localPosition = new Vector3(-40, -35, 0); // a little off
            transform.SetAsFirstSibling(); // below creature
            
            if (thisCardOwnerPlayerStats != null)
            {
                thisCardOwnerPlayerStats.UseMagic(cardStats.magicUse);
            }

            currentLand = tileScript;
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
            thisCardOwnerPlayerStats.currentMagic += cardStats.magicUse; // give back magicUse
        }

        protected override void DetachFromTile()
        {
            currentLand.building = null;
            base.DetachFromTile();
        }
    }
}