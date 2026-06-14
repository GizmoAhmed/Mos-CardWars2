using System;
using System.Collections.Generic;
using AbilityEvents;
using Mirror;
using Modal;
using UnityEngine;

namespace PlayerStuff
{
    public class PlayerCardTracker : NetworkBehaviour
    {
        [SerializeField] private List<GameObject> _serverDiscardedCards = new List<GameObject>();
        [SerializeField] private List<GameObject> _serverHandContents = new List<GameObject>();

        public int numOfCardsDrawnThisTurn;
        
        /// <summary>
        /// Called when a card is discarded - tracked on server
        /// </summary>
        [Server]
        public void Server_TrackDiscard(GameObject card)
        {
            _serverDiscardedCards.Add(card);
            Debug.Log($"[Server] Tracked discard: {card.name}, " +
                      $"total: {_serverDiscardedCards.Count}");
        }
        
        /// <summary>
        /// Server gets last discarded card directly
        /// </summary>
        [Server]
        public GameObject Server_GetLastDiscard()
        {
            if (_serverDiscardedCards.Count == 0)
                return null;
        
            // Find last non-null card (some might have been destroyed)
            for (int i = _serverDiscardedCards.Count - 1; i >= 0; i--)
            {
                if (_serverDiscardedCards[i] != null)
                    return _serverDiscardedCards[i];
            
                // todo if you want it removed
                
                // _serverDiscardedCards.RemoveAt(i);
            }
        
            return null;
        }
        
        [Server]
        public void Server_TrackAddToHand(GameObject card)
        {
            _serverHandContents.Add(card);
            /*Debug.Log($"[Server] Tracked add to hand: {card.name}, " +
                      $"total: {_serverHandContents.Count}");*/

            numOfCardsDrawnThisTurn++;
        }

        [Server]
        public void Server_RemoveFromHand(GameObject card)
        {
            _serverHandContents.Remove(card);
            /*Debug.Log($"[Server] removing {card.name} from hand tracker, " +
                      $"total: {_serverHandContents.Count}");*/
        }

        [Server]
        public void GlobalBroadcastAddToHand(GameObject card)
        {
            if (GlobalAbilityEventManager.GlobalAbilityManagerInstance != null)
            {
                AbilityEventData addToHandData = new AbilityEventData
                (
                    type: AbilityEventType.AnyAddCardToHand,
                    t: card
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
            // todo among other things
        }
    }
}
