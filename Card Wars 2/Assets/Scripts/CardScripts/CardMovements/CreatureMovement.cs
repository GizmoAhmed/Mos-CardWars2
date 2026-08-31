using AbilityEvents;
using CardScripts.CardData;
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

        protected override bool ValidPlacement(Tile tile)
        {
            // if can't get passed global checks, abort
            if (!base.ValidPlacement(tile))
                return false;

            if (!(tile is MiddleTile midTile)) // has to be middle tile
                return false;

            // get correct tile depending on which client is calling
            MiddleTile logTile = GetServerTileForClient(midTile) as MiddleTile;
            
            // the tile has to be on your client side, the bottom row
            // AND the tile has to not have a creature on it
            return midTile.clientTileOwner && logTile.logicalCreature == null;
        }

        [Command] // not needed...todo for now
        protected override void CmdPlaceCardOnTile(GameObject tile)
        {
            // add bases stats to score and add soul...
            thisCardOwnerPlayerStats.AddPlayerScore(CreatureStats.score);
            
            thisCardOwnerPlayerStats.currentSoul -= CreatureStats.soulUse;
            
            // when placed set floop quantity
            CreatureStats.floopsLeft = CreatureStats.maxFloops;
            // floops may get added on placement abilities below
            
            // track placed creature
            thisCardOwnerPlayerStats.GetComponent<PlayerCardTracker>().Server_TrackTilePlacement(gameObject);
            
            // ... then broadcast, placement abilities need the initial player score to be added first
            base.CmdPlaceCardOnTile(tile); 
            
            // above broadcasts all card placements, since all card types call the base above
            // this call below narrows a call down to just creatures, that way if a charm or something only cares about creatures, it doesn't have to do the wide search above, you feel me?\
            GlobalBroadcast_AnyCreaturePlacement();
        }
        
        // you tell the global instance that a card placed, which lets EVERYONE know to trigger their abilities if they care
        private void GlobalBroadcast_AnyCreaturePlacement()
        {
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance != null)
            {
                AbilityEventData cardPlaceData = new AbilityEventData(
                    AbilityEventType.AnyCreaturePlaced,
                    gameObject
                );

                // tell event manager to tell everyone (that cares) that this card was placed
                GlobalAbilityEventManager.GlobalAbilityManagerInstance.TriggerEvents_ForAllSubscribersOfType(
                    cardPlaceData);
            }
            else
            {
                Debug.LogError($"{gameObject.name} couldn't find the ability event manager");
            }
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

            middleTile.logicalCreature = gameObject;
        }

        [Server]
        protected override void ClearLogicalReference_OnTile(Tile tile)
        {
            MiddleTile middleTile = tile as MiddleTile;
            
            if (middleTile.logicalCreature == gameObject)
            {
                middleTile.logicalCreature = null;
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
            visualTile.creatureVisual = gameObject;
            transform.SetParent(visualTile.transform, false);
            transform.localPosition = Vector3.zero;
            transform.SetAsFirstSibling();

            // Update visual reference
            thisCardsVisualTile = visualTile;
        }

        [Server]
        public override void BurnCard()
        {
            // todo creature burned broadcast, below are the remnants of that
            // you'd have to get rid of the GlobalBroadcastBurn(cardToBurn); in player stats for this to work, and then make a global broadcast for all 5 card types
            
           //  GlobalBroadcast_AnyCreatureBurn(gameObject);
           LocalWhisperCreatureBurn(GetLogicalTile());
           
           base.BurnCard(); // server discard
        }

        private void LocalWhisperCreatureBurn(Tile tile)
        {
            TileEventManager tileEventManager = tile.gameObject.GetComponent<TileEventManager>();

            tileEventManager.OnCreatureBurnedOnTile(gameObject);
        }

        /*private void GlobalBroadcast_AnyCreatureBurn(GameObject burnedCard)
        {
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance != null)
            {
                AbilityEventData burnData = new AbilityEventData(
                    AbilityEventType.AnyCreatureBurned,
                    burnedCard
                );

                // tell event manager to tell everyone (that cares) that a card was burned
                GlobalAbilityEventManager.GlobalAbilityManagerInstance.TriggerEvents_ForAllSubscribersOfType(
                    burnData);
            }
            else
            {
                Debug.LogError($"{gameObject.name} couldn't find the ability event manager");
            }
        }*/

        [Server]
        public override void ServerDiscard()
        {
            // if being discarded from the field, returning magic
            if (cardState == CardState.Field)
            {
                // remove tracking on placed creature
                thisCardOwnerPlayerStats.GetComponent<PlayerCardTracker>().Server_RemoveTilePlacement(gameObject);
                
                ReturnSoulAndScore();
                
                // todo reset blockers, immortality, and whatever else is needed
            }
            
            // remove all runes
            GetComponentInChildren<RuneSlots>().UnbindAllRunes();
            
            // tell tile that a card was discarded on it
            LocalWhisperCardDiscard(GetLogicalTile());
            
            base.ServerDiscard();
        }

        private void ReturnSoulAndScore()
        {
            // give back soulUse
            thisCardOwnerPlayerStats.currentSoul += cardStats.soulUse; 
            // cardStats.UpdateSyncSoulToPlayer(cardStats.soulUse);
            
            thisCardOwnerPlayerStats.playerTotalScore -= CreatureStats.score; // give back score
        }
        
        private void LocalWhisperCardDiscard(Tile tile)
        {
            TileEventManager tileEventManager = tile.gameObject.GetComponent<TileEventManager>();

            // tileManager handles creating and broadcasting the AbilityEventData
            tileEventManager.OnCardDiscardFromTile(gameObject);
        }

        protected override void DetachFromTile()
        {
            ((MiddleTile)thisCardsVisualTile).creatureVisual = null;
            base.DetachFromTile();
        }
    }
}