using AbilityEvents;
using CardScripts.CardData;
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
            
            if (!(tile is CharmTile charmTile)) // has to be middle tile
                return false;

            // return true if one of your own tile, the close ones
            return charmTile.tileOwner;
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
            CharmTile charmTile = tile as CharmTile;

            charmTile.AddCharm(gameObject);
        }

        [ClientRpc] // assume valid, so don't worry about ok to place or not
        protected override void RpcPlaceCardOnTile(GameObject tileObj)
        {
            base.RpcPlaceCardOnTile(tileObj);

            CharmTile charmTileScript = tileObj.GetComponent<CharmTile>();
            CharmTile visualCharmTile = charmTileScript;

            // VISUAL MIRRORING: If opponent's charm, show on their charm zone
            if (!isOwned)
            {
                GameObject acrossTile = charmTileScript.across;
                visualCharmTile = acrossTile.GetComponent<CharmTile>();
            }

            // Visual positioning (keep world space for charms)
            transform.SetParent(visualCharmTile.transform, true);
        
            // Update visual reference
            currentTileVisual = visualCharmTile;
        
            // Use magic
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
            // todo prolly shouldn't be using the visual here
            CharmTile charmTileScript = currentTileVisual.GetComponent<CharmTile>();

            charmTileScript.RemoveCharm(gameObject);

            base.DetachFromTile();
        }
    }
}