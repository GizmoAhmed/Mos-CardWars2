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

        protected override bool ValidPlacement(Tile land)
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

            Tile tileScript = tileObj.GetComponent<Tile>();

            if (!isOwned)
            {
                tileObj = tileScript.across;
                tileScript = tileObj.GetComponent<Tile>();
            }

            tileScript.creature = gameObject; // set tiles creature as this

            transform.SetParent(tileObj.transform, false); // set card as child of tile
            transform.localPosition = Vector3.zero; // a little off
            transform.SetAsFirstSibling(); // above building
            
            if (thisCardOwnerPlayerStats != null)
            {
                thisCardOwnerPlayerStats.AddScore(CreatureStats.score);
                thisCardOwnerPlayerStats.UseMagic(CreatureStats.magicUse);
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
            thisCardOwnerPlayerStats.currentMagic += cardStats.magicUse; // give back magicUse
            thisCardOwnerPlayerStats.score -= CreatureStats.score; // give back score
        }

        protected override void DetachFromTile()
        {
            currentLand.creature = null;
            base.DetachFromTile();
        }
    }
}