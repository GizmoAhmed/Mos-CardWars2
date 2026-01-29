using CardScripts.CardStatss;
using Lands;
using Mirror;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class CreatureMovement : BaseMovement // these are creatures and buildings
    {
        private CreatureStats CreatureStats => cardStats as CreatureStats;

        protected override bool ValidPlacement(MiddleLand land)
        {
            // if can't get passed global checks, abort
            if (!base.ValidPlacement(land))
                return false;
            
            return land.tileOwner && land.creature == null && !land.IsOccupied;
        }

        [ClientRpc] // assume valid, so don't worry about ok to place or not
        protected override void RpcPlaceCardOnTile(GameObject tileObj)
        {
            base.RpcPlaceCardOnTile(tileObj);

            MiddleLand tileScript = tileObj.GetComponent<MiddleLand>();

            if (!isOwned)
            {
                tileObj = tileScript.across;
                tileScript = tileObj.GetComponent<MiddleLand>();
            }

            tileScript.creature = gameObject; // set tiles creature as this

            transform.SetParent(tileObj.transform, false); // set card as child of tile
            transform.localPosition = Vector3.zero; // a little off
            transform.SetAsFirstSibling(); // above building

            if (CreatureStats?.thisCardOwner != null)
            {
                CreatureStats.thisCardOwner.AddScore(CreatureStats.score);
            }

            if (CreatureStats?.thisCardOwner != null)
            {
                CreatureStats.thisCardOwner.UseMagic(CreatureStats.magicUse);
            }

            currentLand = tileScript;
        }

        protected override void Discard()
        {
            if (cardState == CardState.Field)
            {
                // change player sync vars requires server call, 
                ReturnMagicAndScore(); 
            }

            base.Discard();
        }

        [Command]
        private void ReturnMagicAndScore()
        {
            CreatureStats.thisCardOwner.currentMagic += cardStats.magicUse; // give back magicUse
            CreatureStats.thisCardOwner.score -= CreatureStats.score; // give back score
        }

        protected override void DetachFromTile()
        {
            currentLand.creature = null;
            base.DetachFromTile();
        }
    }
}