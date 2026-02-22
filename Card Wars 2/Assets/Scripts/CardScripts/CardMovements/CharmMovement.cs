using Mirror;
using PlayerStuff;
using Tiles;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class CharmMovement : CardMovement
    {
        protected override bool ValidPlacement(Tile land)
        {
            // if can't get passed global checks, abort
            if (!base.ValidPlacement(land))
                return false;

            // return true if one of your own tile, the close ones
            return land.tileOwner;
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
            charmTileScript.InUseCharms.Add(gameObject);
            
            if (thisCardOwnerPlayerStats != null)
            {
                thisCardOwnerPlayerStats.UseMagic(cardStats.magicUse);
            }

            currentLand = charmTileScript;
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
            thisCardOwnerPlayerStats.currentMagic += cardStats.magicUse; // give back magicUse
        }

        protected override void DetachFromTile()
        {
            CharmTile charmTileScript = currentLand.GetComponent<CharmTile>();

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