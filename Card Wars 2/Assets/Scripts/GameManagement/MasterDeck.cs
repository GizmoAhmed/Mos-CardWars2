using System;
using System.Collections.Generic;
using CardScripts.CardData;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using Mirror;
using PlayerStuff;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

namespace GameManagement
{
    /// <summary>
    /// Serves as a place to generate cards by id
    /// </summary>
    public class MasterDeck : NetworkBehaviour
    {
        [Header("Deck Mode")]
        [Tooltip("ON: players draw from the finite Debug Deck (cards are consumed). " +
                 "OFF: players draw from the full master pool and it never runs out.")]
        public bool debugMode = true;

        /// <summary>Debug mode only counts in the editor or development builds.</summary>
        public bool DebugDeckActive => debugMode && (Application.isEditor || Debug.isDebugBuild);

        public List<CardDataSO> debugDeck;

        private readonly Dictionary<string, CardDataSO> _allCardsDict = new Dictionary<string, CardDataSO>();

        [Header("THIS ONE AUTO-POPULATES DON'T TOUCH IT")]
        public List<CardDataSO> masterDeckList;

        [Serializable] // editable in inspector
        public struct RarityWeight
        {
            public CardDataSO.Rarity rarity;
            [Min(0)] public float weight;
            [Min(0)] public float burnReturnMultiplier;  
        }

        [Header("Rarity Odds (relative weights, don't need to sum to 100)")]
        public List<RarityWeight> rarityWeights = new List<RarityWeight>
        {
            new RarityWeight { rarity = CardDataSO.Rarity.Common, weight = 62, burnReturnMultiplier = 1f },
            new RarityWeight { rarity = CardDataSO.Rarity.Uncommon, weight = 25, burnReturnMultiplier = 1.5f },
            new RarityWeight { rarity = CardDataSO.Rarity.Rare, weight = 10, burnReturnMultiplier = 2f },
            new RarityWeight { rarity = CardDataSO.Rarity.Epic, weight = 3, burnReturnMultiplier = 3.5f },
        };

        // Built in PopulateLocal(): rarity -> every card of that tier
        private readonly Dictionary<CardDataSO.Rarity, List<CardDataSO>> _cardsByRarity
            = new Dictionary<CardDataSO.Rarity, List<CardDataSO>>();

        [Tooltip("The name of the leaf node folder where all of the cards are stored")]
        private readonly string cardSOLeafFolderName = "CardSOs";

        [Header("Base Card GameObjects")] public GameObject creatureCard;
        public GameObject buildingCard;
        public GameObject spellCard;
        public GameObject runeCard;
        public GameObject charmCard;

        /// <summary>The list a player's deck should start as.</summary>
        public List<CardDataSO> GetStartingDeck()
        {
            if (masterDeckList.Count == 0)
            {
                Debug.LogError("There are no cards in the master deck");
            }

            return DebugDeckActive ? debugDeck : masterDeckList;
        }

        // Server: populate immediately so the deck is ready on the very next line
        [Server]
        public void InitMasterDeck()
        {
            if (creatureCard == null || buildingCard == null || spellCard == null ||
                runeCard == null || charmCard == null)
            {
                Debug.LogError($"Base Cards were not set on {gameObject.name} in the editor");
                return;
            }

            PopulateLocal();
        }

        // Joiner: populate when this object spawns on their machine
        public override void OnStartClient()
        {
            base.OnStartClient();

            // On the host, the server already populated via InitMasterDeck()
            if (isServer) return;

            PopulateLocal();
        }

        private void PopulateLocal()
        {
            _allCardsDict.Clear(); // makes repeat calls safe
            AutoPopulateMasterDeck();
            CreateDict_FromMasterDeck();
            BuildRarityGroups();
        }
        
        private void BuildRarityGroups()
        {
            _cardsByRarity.Clear();

            foreach (CardDataSO card in masterDeckList)
            {
                if (!_cardsByRarity.TryGetValue(card.cardRarity, out List<CardDataSO> group))
                {
                    group = new List<CardDataSO>();
                    _cardsByRarity[card.cardRarity] = group;
                }

                group.Add(card);
            }
        }
        
        /// <summary>
        /// Given the cards rarity, multiple money gained from burn return by that cards rarity
        /// Rarer cards give more money when burned
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int GetBurnReturn(CardDataSO data)
        {
            foreach (RarityWeight rw in rarityWeights)
            {
                if (rw.rarity == data.cardRarity)
                    return Mathf.RoundToInt(data.burnReturn * rw.burnReturnMultiplier);
            }

            return data.burnReturn;   // rarity not configured: no multiplier
        }
        
        /// <summary>
        /// This catches duplicate tiers or all-zero weights while you’re editing,
        /// since a list (unlike a dictionary) lets you add the same rarity twice by accident
        /// </summary>
        private void OnValidate()
        {
            var duplicates = rarityWeights
                .GroupBy(r => r.rarity)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var rarity in duplicates)
                Debug.LogWarning($"MasterDeck: rarity {rarity} appears more than once in Rarity Weights");

            if (rarityWeights.Count > 0 && rarityWeights.All(r => r.weight <= 0))
                Debug.LogWarning("MasterDeck: all rarity weights are zero, nothing can be drawn");
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
                Debug.LogError(
                    $"GetCardByID({cardID}) --> _allCardsDict is null, can't retrieve a card with id:  {cardID}");
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

        /// <summary>
        /// Find, Instantiate, then spawn the card.
        /// Also sets the stats and moves card to hand
        /// </summary>
        /// <param name="cardID">Id of card being created and spawned</param>
        /// <param name="playerStats">This is the player drawing</param>
        [Server]
        public GameObject CreateThenSpawnCard(string cardID, PlayerStats playerStats, bool isDuplicate = false)
        {
            CardDataSO cardData = GetCardByID(cardID);

            GameObject cardObj = CreateCard(cardData);

            CardMovement move = cardObj.GetComponent<CardMovement>();
            move.SetOwningPlayer(playerStats);

            //// lesson here:
            // this line below used master decks connection, which is a server object,
            // so isOwned is never set on card...
            // NetworkServer.Spawn(cardObj, connectionToClient);

            // ...instead, use the player's connection,
            // so isOwned can be set on the spawned card according to the player
            NetworkIdentity playerIdentity = playerStats.GetComponent<NetworkIdentity>();
            NetworkServer.Spawn(cardObj, playerIdentity.connectionToClient);

            // set card data with network ON, since spawning for both clients on server
            // goes after SPAWN, because needs to be spawned to use the rpc within SetCardData
            CardStats cardStats = cardObj.GetComponent<CardStats>();
            cardStats.SetAndApplyCardData(cardData, serverCall: true);

            Player player = playerStats.GetComponent<Player>();

            if (player != null)
            {
                player.cardPlacer.Rpc_MoveCardToHand(cardObj);

                // add to card tracker for this player
                player.playerCardTracker.Server_TrackAddToHand(cardObj);

                // broadcast add to hand
                player.playerCardTracker.GlobalBroadcastAddToHand(cardObj, isDuplicate);
            }
            else
            {
                Debug.LogError($"Player was not found for {cardObj.name}.");
            }

            move.cardState = CardMovement.CardState.Hand;

            return cardObj;
        }

        /// <summary>
        /// Creates a card using Prefab cards as bodies (from game manager) and fills them with souls in the form of CardDataSO
        /// First step prior to spawning on server, since draw modal doesn't require spawn, you can just create them here
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public GameObject CreateCard(CardDataSO data)
        {
            GameObject card;

            switch (data)
            {
                case CreatureDataSO:
                    card = creatureCard;
                    break;
                case BuildingDataSO:
                    card = buildingCard;
                    break;
                case SpellDataSO:
                    card = spellCard;
                    break;
                case CharmDataSO:
                    card = charmCard;
                    break;
                case RuneDataSO:
                    card = runeCard;
                    break;
                default:
                    Debug.LogError($"{data.GetType()}: card data is null or the base class. Can't create card");
                    return null;
            }

            GameObject cardInstance = Instantiate(card);

            // cardInstance.GetComponent<CardStats>().SetCardData(data);

            return cardInstance;
        }
        
        /// <summary>
        /// Weighted random card: rolls a rarity tier first, then picks uniformly within it.
        /// </summary>
        /// <param name="excludeCardIDs">Pass cards to exclude (e.g. already offered in the draw modal).</param>
        /// <returns>Returned Random Card</returns>
        private CardDataSO GetWeightedRandomCard(ICollection<string> excludeCardIDs = null)
        {
            // Only tiers that still have at least one eligible card take part in the roll
            var eligible = new List<(CardDataSO.Rarity rarity, float weight, List<CardDataSO> cards)>();
            float totalWeight = 0f;

            foreach (RarityWeight rw in rarityWeights)
            {
                if (rw.weight <= 0f) continue;
                if (!_cardsByRarity.TryGetValue(rw.rarity, out List<CardDataSO> tierCards)) continue;

                List<CardDataSO> available = excludeCardIDs == null
                    ? tierCards
                    : tierCards.Where(c => !excludeCardIDs.Contains(c.cardID)).ToList();

                if (available.Count == 0) continue;

                eligible.Add((rw.rarity, rw.weight, available));
                totalWeight += rw.weight;
            }

            if (eligible.Count == 0)
            {
                Debug.LogWarning("GetWeightedRandomCard: no eligible cards to draw");
                return null;
            }

            // Roll a point on the number line [0, totalWeight) and see which tier it lands in
            float roll = Random.Range(0f, totalWeight);

            foreach (var tier in eligible)
            {
                roll -= tier.weight;
                if (roll < 0f)
                    return tier.cards[Random.Range(0, tier.cards.Count)];
            }

            // Floating point safety net: fall back to the last tier
            var last = eligible[eligible.Count - 1];
            return last.cards[Random.Range(0, last.cards.Count)];
        }

        /// <summary>
        /// Several unique weighted cards, for the draw modal offer.
        /// </summary>
        public List<string> GetWeightedUniqueCardIDs(int count)
        {
            var picked = new List<string>();

            for (int i = 0; i < count; i++)
            {
                CardDataSO card = GetWeightedRandomCard(excludeCardIDs: picked);
                if (card == null) break;   // pool exhausted
                picked.Add(card.cardID);
            }

            return picked;
        }
    }
}