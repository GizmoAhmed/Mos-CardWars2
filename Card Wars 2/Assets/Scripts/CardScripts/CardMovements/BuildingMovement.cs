using Lands;
using Mirror;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class BuildingMovement : BaseMovement
    {
        protected override bool ValidPlacement(MiddleLand land)
        {
            // if can't get passed global checks, abort
            if (!base.ValidPlacement(land))
                return false;

            // return true if one of your own lands, the close ones
            return land.tileOwner && land.building == null;
        }

        [ClientRpc] // assume valid, so don't worry about ok to place or not
        protected override void RpcPlaceCardOnTile(GameObject tileObj)
        {
            base.RpcPlaceCardOnTile(tileObj);

            MiddleLand tileScript = tileObj.GetComponent<MiddleLand>();

            var x = cardStats.thisCardOwner;

            if (!isOwned)
            {
                tileObj = tileScript.across;
                tileScript = tileObj.GetComponent<MiddleLand>();
            }

            tileScript.building = gameObject; // set tiles building as this

            transform.SetParent(tileObj.transform, false); // set card as child of tile
            transform.localPosition = new Vector3(-40, -35, 0); // a little off
            transform.SetAsFirstSibling(); // below creature

            if (cardStats?.thisCardOwner != null)
            {
                cardStats.thisCardOwner.UseMagic(cardStats.magicUse);
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
            cardStats.thisCardOwner.currentMagic += cardStats.magicUse; // give back magicUse
        }

        protected override void DetachFromTile()
        {
            currentLand.building = null;
            base.DetachFromTile();
        }
    }
}