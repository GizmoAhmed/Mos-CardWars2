using System;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.CardStatss;
using Mirror;
using Modal;
using UnityEngine;

namespace PlayerStuff
{
    public class PlayerCardTracker : NetworkBehaviour
    {
        [SerializeField] private List<GameObject> serverDiscardedCards = new List<GameObject>();
        [SerializeField] private List<GameObject> serverHandContents = new List<GameObject>();
        [SerializeField] private List<GameObject> serverActiveFieldCards = new List<GameObject>();

        public int numOfCardsDrawnThisTurn;

        public int cardsPlacedThisTurn;
        
        /// <summary>
        /// Called when a card is discarded - tracked on server
        /// </summary>
        [Server]
        public void Server_TrackDiscard(GameObject card)
        {
            serverDiscardedCards.Add(card);
            // Debug.Log($"[Server] Tracked discard: {card.name}. Cards discarded: {_serverDiscardedCards.Count}");
        }
        
        /// <summary>
        /// Server gets last discarded card directly
        /// </summary>
        [Server]
        public GameObject Server_GetLastDiscard()
        {
            if (serverDiscardedCards.Count == 0)
                return null;
        
            // Find last non-null card (some might have been destroyed)
            for (int i = serverDiscardedCards.Count - 1; i >= 0; i--)
            {
                if (serverDiscardedCards[i] != null)
                    return serverDiscardedCards[i];
            
                // todo if you want it removed
                
                // _serverDiscardedCards.RemoveAt(i);
            }
        
            return null;
        }
        
        [Server]
        public void Server_TrackAddToHand(GameObject card)
        {
            serverHandContents.Add(card);
            /*Debug.Log($"[Server] Tracked add to hand: {card.name}, " +
                      $"total: {_serverHandContents.Count}");*/

            numOfCardsDrawnThisTurn++;
        }

        [Server]
        public void Server_RemoveFromHand(GameObject card)
        {
            serverHandContents.Remove(card);
            /*Debug.Log($"[Server] removing {card.name} from hand tracker, " +
                      $"total: {_serverHandContents.Count}");*/
        }

        [Server]
        public void GlobalBroadcastAddToHand(GameObject card, bool isDuplicate = false)
        {
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance != null)
            {
                AbilityEventData addToHandData = new AbilityEventData(
                    type: AbilityEventType.AnyAddCardToHand,
                    t: card,
                    
                    // prevent infinite loop duplication
                    customData: new Dictionary<string, object>
                    {
                        { "isDuplicate", isDuplicate }
                    }
                    // below broadcast will return to the ability that listens+redraws
                    // and if the above flag is true, won't duplicate a draw again
                );
                
                // tell event manager to tell everyone (that cares) that the turn ended
                GlobalAbilityEventManager.GlobalAbilityManagerInstance
                    .TriggerEvents_ForAllSubscribersOfType(addToHandData);
            }
            else
            {
                Debug.LogError($"<color=Red>Failed Add-To-Hand Broadcast</color> {name} couldn't find the GlobalAbilityEventManage Instance");
            }
        }

        [Server]
        public void Server_EndOfTurnCardTrackerReset()
        {
            numOfCardsDrawnThisTurn = 0;
            cardsPlacedThisTurn = 0;
            // todo among other things

            Server_ResetFloops_OfAllCreatures();
        }

        [Server]
        public void Server_ResetFloops_OfAllCreatures()
        {
            List<CreatureStats> allCreaturesOnField = Server_GetThisPlayersOnFieldCreatures();

            foreach (CreatureStats creatureStats in allCreaturesOnField)
            {
                // Debug.Log($"Tracker resetting <color=orange>{creatureStats.gameObject.name}</color> floops to {creatureStats.maxFloops}");
                creatureStats.ResetFloops();
            }
        }

        [Server]
        public void Server_TrackTilePlacement(GameObject card)
        {
            serverActiveFieldCards.Add(card);
            // Debug.Log($"<color=cyan>Tracker added</color> {card.name} to tile tracker. Current count: {_serverActiveFieldCards.Count}");

            cardsPlacedThisTurn++;
        }
        
        [Server]
        public void Server_RemoveTilePlacement(GameObject card)
        {
            serverActiveFieldCards.Remove(card);
            //Debug.Log($"<color=orange>Tracker removed</color> {card.name} from tile tracker. Current count: {_serverActiveFieldCards.Count}");
        }

        /// <summary>
        /// Get all CreatureStats currently on the field
        /// </summary>
        [Server]
        public List<CreatureStats> Server_GetThisPlayersOnFieldCreatures()
        {
            List<CreatureStats> creatures = new List<CreatureStats>();
    
            foreach (GameObject card in serverActiveFieldCards)
            {
                if (card != null && card.TryGetComponent(out CreatureStats stats))
                {
                    creatures.Add(stats);
                }
            }
    
            return creatures;
        }
        
        [Server]
        public int Server_GetPlayerHandCount()
        {
            return serverHandContents.Count;
        }
    }
}
