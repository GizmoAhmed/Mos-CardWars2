using Lands;
using Mirror;
using PlayerStuff;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class CharmMovement : CardMovement
    {
        protected override bool ValidPlacement(MiddleLand land)
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

            CharmArea charmAreaScript = tileObj.GetComponent<CharmArea>();

            if (!isOwned)
            {
                tileObj = charmAreaScript.across;
                charmAreaScript = tileObj.GetComponent<CharmArea>();
            }

            transform.SetParent(tileObj.transform, true);
            charmAreaScript.InUseCharms.Add(gameObject);
            
            if (thisCardOwnerPlayerStats != null)
            {
                thisCardOwnerPlayerStats.UseMagic(cardStats.magicUse);
            }

            currentLand = charmAreaScript;
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
            CharmArea charmAreaScript = currentLand.GetComponent<CharmArea>();

            if (charmAreaScript.InUseCharms.Contains(gameObject))
            {
                charmAreaScript.InUseCharms.Remove(gameObject);
            }
            else
            {
                Debug.LogError("attempted to detach a charm that isn't listed in the charm list");
            }

            base.DetachFromTile();
        }
    }
}