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
        
        [Server]
        public void InitializeDeck(List<CardDataSO> deck)
        {
            myDeckCardIDs.Clear();

            foreach (CardDataSO card in deck)
            {
                myDeckCardIDs.Add(card.cardID);
            }
        }

        [Server]
        private void DrawSpawnCard_ByID(string cardID)
        {
            int index = myDeckCardIDs.IndexOf(cardID);

            if (index < 0)
            {
                Debug.LogWarning($"Card {cardID} not in deck");
                return;
            }

            CardDataSO cardData = _masterDeck.GetCardByID(cardID);

            GameObject cardObj = _masterDeck.CreateCard(cardData);
            
            CardMovement move = cardObj.GetComponent<CardMovement>();
            PlayerStats stats = GetComponentInParent<PlayerStats>();
            move.SetOwningPlayer(stats);

            NetworkServer.Spawn(cardObj, connectionToClient);
            
            // set card data with network ON, since spawning for both clients on server
            // goes after SPAWN, because needs to be spawned to use the rpc within SetCardData
            CardStats cardStats = cardObj.GetComponent<CardStats>();
            cardStats.SetCardData(cardData, serverCall: true);

            Player player = GetComponentInParent<Player>();

            if (player != null)
            {
                player.cardPlacer.MoveCardToHand(cardObj);
            }
            else
            {
                Debug.LogError($"Player was not found for {cardObj.name}.");
            }

            move.cardState = CardMovement.CardState.Hand;

            myDeckCardIDs.RemoveAt(index);
        }

        [Command] // called from in-scene button click
        public void CmdDrawCard()
        {
            DrawTopCardFromDeck(connectionToClient);
        }

        [Server]
        private void DrawTopCardFromDeck(NetworkConnectionToClient conn)
        {
            if (myDeckCardIDs.Count == 0)
            {
                Debug.LogWarning("Empty Deck");
                return;
            }

            string cardID = myDeckCardIDs[0];

            DrawSpawnCard_ByID(cardID);
        }

        [Command] // called from in-scene button click
        public void CmdDrawAllFromDeck()
        {
            DrawAllFromDeck(connectionToClient);
        }

        [Server]
        private void DrawAllFromDeck(NetworkConnectionToClient conn)
        {
            if (myDeckCardIDs.Count == 0)
            {
                Debug.LogWarning("Empty Deck");
                return;
            }
            
            // iterate backwards because sync list
            for (int i = myDeckCardIDs.Count - 1; i >= 0; i--)
            {
                DrawSpawnCard_ByID(myDeckCardIDs[i]);
            }
        }
        
        /// <summary>
        /// Draw a specific card by ID (called when player selects from preview)
        /// </summary>
        [Command(requiresAuthority = false)]
        public void CmdDrawCardByID(string cardID, NetworkConnectionToClient sender = null)
        {
            DrawSpawnCard_ByID(cardID);
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
            List<string> randomCardNames = GetRandomUniqueCardNames(offering);

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
                cardStats.SetCardData(cardData, serverCall: false);

                // move to drawmodal
                previewCard.transform.SetParent(drawModal.cardGroupTransform, false);

                // init card
                // previewCard.GetComponent<CardStats>().InitializeCard();

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
        private List<string> GetRandomUniqueCardNames(int count)
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
    }
}