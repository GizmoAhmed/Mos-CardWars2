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

            // return true if one of your own tile, the close ones
            return tile.tileOwner;
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
            visualCharmTile.InUseCharms.Add(gameObject);
        
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
            CharmTile charmTileScript = currentTileVisual.GetComponent<CharmTile>();

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