using System;
using System.Collections.Generic;
using Mirror;
using Modal;
using UnityEngine;

namespace PlayerStuff
{
    public class CardTracker : NetworkBehaviour
    {
        private List<GameObject> _serverDiscardedCards = new List<GameObject>();
        
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
    }
}
