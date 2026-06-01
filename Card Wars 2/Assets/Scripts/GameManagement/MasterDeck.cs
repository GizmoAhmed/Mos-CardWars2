using System;
using System.Collections.Generic;
using CardScripts.CardData;
using Mirror;
using UnityEngine;

namespace GameManagement
{
    /// <summary>
    /// Serves as a place to generate cards by id
    /// </summary>
    public class MasterDeck : NetworkBehaviour
    {
        public List<CardDataSO> debugDeck;

        private readonly Dictionary<string, CardDataSO> _allCardsDict = new Dictionary<string, CardDataSO>();

        public List<CardDataSO> masterDeckList;

        [Tooltip("The name of the leaf node folder where all of the cards are stored")]
        private readonly string cardSOLeafFolderName = "CardSOs";

        // Initialize on server
        [Server]
        public void InitMasterDeck()
        {
            RpcInitMasterDeck();
        }

        [ClientRpc]
        private void RpcInitMasterDeck()
        {
            AutoPopulateMasterDeck();
            CreateDict_FromMasterDeck();
        }

        /// <summary>
        /// Automatically finds all CardDataSO assets in Assets/Resources/CardSOs folder
        /// </summary>
        private void AutoPopulateMasterDeck()
        {
            masterDeckList.Clear();
            
            CardDataSO[] allCards = Resources.LoadAll<CardDataSO>(cardSOLeafFolderName);

            if (allCards.Length == 0)
            {
                Debug.LogError(
                    $"No cards found in Resources/{cardSOLeafFolderName}! Make sure your cards are in that folder.");
                return;
            }

            masterDeckList.AddRange(allCards);

            Debug.Log($"Auto-populated master deck with {masterDeckList.Count} cards");
        }

        /// <summary>
        /// Creates a dictionary out of the master deck for easy look up when a card is asked to be generated
        /// </summary>
        private void CreateDict_FromMasterDeck()
        {
            if (masterDeckList.Count == 0)
            {
                Debug.LogWarning("Master Deck is empty!");
                return;
            }

            foreach (CardDataSO card in masterDeckList)
            {
                // adds, if possible. if not, log warning
                if (!_allCardsDict.TryAdd(card.cardID, card))
                {
                    Debug.LogWarning($"Duplicate card ID: {card.cardID}");
                }
            }
        }

        public CardDataSO GetCardByID(string cardID)
        {
            if (_allCardsDict == null)
            {
                Debug.LogError($"GetCardByID({cardID}) --> _allCardsDict is null, can't retrieve a card");
                return null;
            }

            if (_allCardsDict.Count == 0)
            {
                Debug.LogError(
                    $"GetCardByID({cardID}) --> _allCardsDict is empty, which means master deck wasn't populated, check that");
                return null;
            }

            if (_allCardsDict.TryGetValue(cardID, out CardDataSO card))
            {
                return card;
            }

            Debug.LogError($"Card not found: {cardID}");
            return null;
        }
    }
}