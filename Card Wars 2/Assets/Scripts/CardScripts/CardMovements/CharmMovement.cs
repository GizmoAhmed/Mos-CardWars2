using Mirror;
using PlayerStuff;
using Tiles;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class CharmMovement : CardMovement
    {
        protected override bool ValidPlacement(Tile tile)
        {
            // if can't get passed global checks, abort
            if (!base.ValidPlacement(tile))
                return false;

            // return true if one of your own tile, the close ones
            return tile.tileOwner;
        }
        
        [Command]
        protected override void CmdPlaceCardOnTile(GameObject tile)
        {
            base.CmdPlaceCardOnTile(tile);
        }

        [ClientRpc] // assume valid, so don't worry about ok to place or not
        protected override void RpcPlaceCardOnTile(GameObject tileObj)
        {
            base.RpcPlaceCardOnTile(tileObj);

            CharmTile charmTileScript = tileObj.GetComponent<CharmTile>();

            if (!isOwned)
            {
                tileObj = charmTileScript.across;
                charmTileScript = tileObj.GetComponent<CharmTile>();
            }

            transform.SetParent(tileObj.transform, true);
            charmTileScript.InUseCharms.Add(gameObject); // add to charms in use todo prolly gonna get deprecated
            
            if (thisCardOwnerPlayerStats != null)
            {
                thisCardOwnerPlayerStats.UseMagic(cardStats.soulUse);
            }
        }

        protected override void Discard()
        {
            if (cardState == CardState.Field)
            {
                // change player sync vars requires server call, if the client running is the server, do so
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
            CharmTile charmTileScript = currentTile.GetComponent<CharmTile>();

            if (charmTileScript.InUseCharms.Contains(gameObject))
            {
                charmTileScript.InUseCharms.Remove(gameObject);
            }
            else
            {
                Debug.LogError("attempted to detach a charm that isn't listed in the charm list");
            }

            base.DetachFromTile();
        }
    }
}