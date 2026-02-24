using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CardScripts;
using CardScripts.CardData;
using CardScripts.CardDisplays;
using CardScripts.CardMovements;
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
        public readonly SyncList<string> myDeckCardIDs = new SyncList<string>();

        public DrawModal drawModal;

        [Server]
        public void InitializeDeck(List<CardDataSO> deck)
        {
            myDeckCardIDs.Clear();

            foreach (CardDataSO card in deck)
            {
                myDeckCardIDs.Add(card.cardID);
            }
        }

        void Start()
        {
            drawModal = FindObjectOfType<GameManager>().gmVisibleDrawModal.GetComponent<DrawModal>();

            if (drawModal == null)
            {
                Debug.LogError($"DrawModal is null on {gameObject.name}");
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

            GameManager gm = FindObjectOfType<GameManager>();
            CardDataSO cardData = gm.GetCardByID(cardID);

            GameObject cardObj = CreateCard(cardData);

            CardMovement move = cardObj.GetComponent<CardMovement>();
            PlayerStats stats = GetComponentInParent<PlayerStats>();
            move.SetOwningPlayer(stats);

            NetworkServer.Spawn(cardObj, connectionToClient);

            Player player = GetComponentInParent<Player>();
            if (player != null)
            {
                player.cardPlacer.MoveCardToHand(cardObj);
            }

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

        private GameObject CreateCard(CardDataSO data)
        {
            GameObject card;

            GameManager gm = FindObjectOfType<GameManager>();

            if (data is CreatureDataSO) card = gm.creatureCard;
            else if (data is BuildingDataSO) card = gm.buildingCard;
            else if (data is SpellDataSO) card = gm.spellCard;
            else if (data is CharmDataSO) card = gm.charmCard;
            else if (data is RuneDataSO) card = gm.runeCard;
            else
            {
                Debug.LogError($"{data.GetType()}: card data is null or the base class. Can't create card");
                return null;
            }

            GameObject cardInstance = Instantiate(card);

            cardInstance.GetComponent<CardStats>().SetCardData(data);

            return cardInstance;
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
            GameManager gm = FindObjectOfType<GameManager>();

            drawModal.UpdatePicksLeft(choice);

            foreach (string cardName in randomCardNames)
            {
                CardDataSO cardData = gm.GetCardByID(cardName);

                if (cardData == null)
                {
                    Debug.LogError($"Card not found: {cardName}");
                    continue;
                }
                
                // creature card
                GameObject previewCard = CreateCard(cardData);

                // move to drawmodal
                previewCard.transform.SetParent(drawModal.cardGroupTransform, false);
                
                // init card
                previewCard.GetComponent<CardStats>().InitializeCard();

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