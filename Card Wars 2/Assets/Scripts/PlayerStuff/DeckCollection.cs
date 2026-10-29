using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CardScripts;
using CardScripts.CardData;
using CardScripts.CardDisplays;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using CardScripts.CardStatss;
using GameManagement;
using Mirror;
using Modal;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerStuff
{
    public class DeckCollection : NetworkBehaviour
    {
        private readonly SyncList<string> myDeckCardIDs = new SyncList<string>();

        public DrawModal drawModal;

        private GameManager _gameManager;
        
        private MasterDeck _masterDeck;

        void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
            
            if (_gameManager == null)
            {
                Debug.LogError($"_gameManager is null on {gameObject.name}");
                return;
            }
            
            drawModal = _gameManager.gmVisibleDrawModal.GetComponent<DrawModal>();

            if (drawModal == null)
            {
                Debug.LogError($"DrawModal is null on {gameObject.name}");
            }
            
            _masterDeck =  FindObjectOfType<MasterDeck>();

            if (_masterDeck == null)
            {
                Debug.LogError($"MasterDeck is null on {gameObject.name}");                
            }
        }
        
        /// <summary>
        /// From GameManager, can either be initialized with debug or master deck
        /// debug mode uses debug deck, which uses cards placed in inspector to make myDeckCardIDs, then removes them as they are drawn
        /// release mode uses master deck directly instead of any manual inspector placement, which doesn't get used up
        /// </summary>
        /// <param name="deck"></param>
        [Server]
        public void InitializeDeck(List<CardDataSO> deck)
        {
            myDeckCardIDs.Clear();

            foreach (CardDataSO card in deck)
            {
                myDeckCardIDs.Add(card.cardID);
            }
        }

        /// <summary>
        /// Uses the MasterDeck to find, create, then spawn the card using the passed card id 
        /// </summary>
        /// <param name="cardID"></param>
        [Server]
        private void DrawCardByID(string cardID)
        {
            _masterDeck.CreateThenSpawnCard(cardID, GetComponentInParent<PlayerStats>());

            // Full pool mode: the deck never shrinks
            if (!_masterDeck.DebugDeckActive) return;

            int index = myDeckCardIDs.IndexOf(cardID);
            if (index < 0)
            {
                Debug.LogWarning($"Card {cardID} not in deck");
                return;
            }
            myDeckCardIDs.RemoveAt(index);
        }
        
        /// <summary>
        /// Debug function: Used for the in-scene button that simply draws a card.
        /// (Isn't in the actual game)
        /// </summary>
        [Command]
        public void CmdDrawCard()
        {
            DrawTopCardFromDeck(connectionToClient);
        }

        [Server]
        private void DrawTopCardFromDeck(NetworkConnectionToClient conn)
        {
            // don't allow this in release mode
            if (!_masterDeck.debugMode) return;

            if (myDeckCardIDs.Count == 0)
            {
                Debug.LogWarning("Empty Deck");
                return;
            }

            string cardID = myDeckCardIDs[0];

            DrawCardByID(cardID);
        }

        [Command] // called from in-scene button click
        public void CmdDrawAllFromDeck()
        {
            DrawAllFromDeck(connectionToClient);
        }

        /// <summary>
        /// Debug function: Used for the in-scene button that draws all cards in deck.
        /// (Isn't in the actual game)
        /// </summary>
        [Server]
        private void DrawAllFromDeck(NetworkConnectionToClient conn)
        {
            // don't allow this in release mode
            if (!_masterDeck.debugMode) return;
            
            if (myDeckCardIDs.Count == 0)
            {
                Debug.LogWarning("Empty Deck");
                return;
            }
            
            // iterate backwards because sync list
            for (int i = myDeckCardIDs.Count - 1; i >= 0; i--)
            {
                DrawCardByID(myDeckCardIDs[i]);
            }
        }
        
        /// <summary>
        /// Draw a specific card by ID.
        /// Called when player selects a card from offer preview
        /// </summary>
        [Command(requiresAuthority = false)]
        public void CmdDrawCardByID(string cardID, NetworkConnectionToClient sender = null)
        {
            DrawCardByID(cardID);
        }

        [Client]
        public void OfferCardsPreview(int choice, int offering)
        {
            if (drawModal == null)
            {
                Debug.LogError($"No DrawModal attached to {gameObject.name}, aborting card preview");
                return;
            }

            drawModal.ClearPreviewCards(); // clear current offering

            // Get random card names from the deck
            // List<string> randomCardNames = GetRandomUniqueCardNames_FromPlayerDeck(offering);
            
            // uses weights to draw based on card rarity
            List<string> randomCardNames = _masterDeck.DebugDeckActive
                ? GetRandomUniqueCardNames_FromPlayerDeck(offering)   // debug deck: unchanged
                : _masterDeck.GetWeightedUniqueCardIDs(offering);     // pool mode: weighted

            drawModal.UpdatePicksLeft(choice);

            foreach (string cardName in randomCardNames)
            {
                CardDataSO cardData = _masterDeck.GetCardByID(cardName);

                if (cardData == null)
                {
                    Debug.LogError($"Card not found: {cardName}");
                    continue;
                }

                // creature card
                GameObject previewCard = _masterDeck.CreateCard(cardData);
                
                // set card data with network OFF, since instancing for just one client at a time
                CardStats cardStats = previewCard.GetComponent<CardStats>();
                cardStats.SetAndApplyCardData(cardData, serverCall: false);

                // move to drawmodal
                previewCard.transform.SetParent(drawModal.cardGroupTransform, false);

                // flip card up
                CardDisplay cardDisplay = previewCard.GetComponent<CardDisplay>();
                cardDisplay.FlipCard(true);

                CardMovement move = previewCard.GetComponent<CardMovement>();

                // change state
                move.cardState = CardMovement.CardState.Preview;
                move.SetOwningDeck(this);
            }
        }

        /// <summary>
        /// Get random unique card names from the deck
        /// </summary>
        private List<string> GetRandomUniqueCardNames_FromPlayerDeck(int count)
        {
            int maxCount = myDeckCardIDs.Count;

            if (count > maxCount)
            {
                Debug.LogWarning($"Requested {count} cards but only {maxCount} available in deck");
                count = maxCount;
            }

            // Create a pool of indices (we still randomize by index position)
            List<int> indexPool = new List<int>();
            for (int i = 0; i < maxCount; i++)
                indexPool.Add(i);

            List<string> result = new List<string>();

            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(0, indexPool.Count);
                int deckIndex = indexPool[randomIndex];

                // Get the card name at this index
                result.Add(myDeckCardIDs[deckIndex]);

                indexPool.RemoveAt(randomIndex);
            }

            return result;
        }

        public void DrawStartingHand(int handSize)
        {
            if (_masterDeck.debugMode)
            {
                Debug.LogWarning(
                    "Attempted to <color=orange>draw starting hand</color> but refused since in debug mode");
                return;
            }

            // weighted random card
            List<string> randomCardNames = 
                _masterDeck.GetWeightedUniqueCardIDs(count: handSize);

            // draw those cards
            foreach (string cardName in randomCardNames)
            {
                CardDataSO cardData = _masterDeck.GetCardByID(cardName);

                DrawCardByID(cardData.cardID);
            }
        }
    }
}