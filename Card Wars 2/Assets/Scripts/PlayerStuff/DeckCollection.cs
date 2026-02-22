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
        // holds the indices of cards in relation to the master deck
        // so player 1 might have [0,1,2,3,4] then when a card gets removed from the deck...
        // ... lets say 1, then it looks for gameManager.masterDeck[1],
        // takes that card, does it what it does with it, then removes 1 from here...
        // [0,2,3,4]
        public readonly SyncList<int> MyDeckIndices = new SyncList<int>();

        private List<CardDataSO> MyDeckCards
        {
            get
            {
                GameManager gm = FindObjectOfType<GameManager>();
                return MyDeckIndices.Select(index => gm.masterDeck[index]).ToList();
            }
        }
        
        [Server]
        public void InitializeDeck(List<CardDataSO> deck)
        {
            GameManager gm = FindObjectOfType<GameManager>();
            MyDeckIndices.Clear();

            int i = 0;
            foreach (CardDataSO card in deck)
            {
                MyDeckIndices.Add(i);
                i++;
            }
        }

        public DrawModal drawModal;
        
        void Start()
        {
            drawModal = FindObjectOfType<GameManager>().gmVisibleDrawModal.GetComponent<DrawModal>();

            if (drawModal == null)
            {
                Debug.LogError($"DrawModal is null on {gameObject.name}");
            }
        }
        
        [Command] // called from in-scene button click
        public void CmdDrawCard()
        {
            DrawCardFromDeck(connectionToClient);
        }

        private void DrawCardFromDeck(NetworkConnectionToClient conn)
        {
            if (MyDeckIndices.Count == 0)
            {
                Debug.LogWarning($"Empty Deck, Player {connectionToClient.connectionId + 1} Can't draw");
                return;
            }

            // just the top of deck for now 
            int randomIndex = 0; // todo randomize deck at start, then you can just draw from top. that way you only random once
        
            GameManager gm = FindObjectOfType<GameManager>();
            CardDataSO drawnCardData = gm.masterDeck[MyDeckIndices[randomIndex]];

            GameObject cardObj = CreateCard(drawnCardData);
            
            // set owner to player who drew it
            CardMovement move = cardObj.GetComponent<CardMovement>();
            PlayerStats stats = GetComponentInParent<PlayerStats>();
            move.SetOwningPlayer(stats);
            
            NetworkServer.Spawn(cardObj, conn);

            Player player = GetComponentInParent<Player>();
            if (player == null)
            {
                Debug.LogError("Player is null, can't draw");
                return;
            }

            player.cardPlacer.MoveCardToHand(cardObj);

            MyDeckIndices.RemoveAt(randomIndex);
        }

        private GameObject CreateCard(CardDataSO data)
        {
            GameObject card;

            GameManager gm = FindObjectOfType<GameManager>();

            if (data is CreatureDataSO)         card = gm.creatureCard;
            else if (data is BuildingDataSO)    card = gm.buildingCard;
            else if (data is SpellDataSO)       card = gm.spellCard;
            else if (data is CharmDataSO)       card = gm.charmCard;
            else if (data is RuneDataSO)        card = gm.runeCard;
            else
            {
                Debug.LogError($"{data.GetType()}: card data is null or the base class. Can't create card");
                return null;
            }

            GameObject cardInstance = Instantiate(card);

            cardInstance.GetComponent<CardStats>().SetCardData(data);
            
            return cardInstance;
        }

        [Client]
        public void PreviewFreeCards()
        {
            if (drawModal == null)
            {
                Debug.LogError($"No DrawModal attached to {gameObject.name}, aborting card preview");
                return;
            }
            
            drawModal.ClearPreviewCards(); // clear cards if anyone for this set of new ones

            PlayerStats stats = GetComponentInParent<PlayerStats>();
            int cardsToShow = stats.freeCardsOffered;

            List<int> indices = GetRandomUniqueIndices(cardsToShow);
            GameManager gm = FindObjectOfType<GameManager>();

            foreach (int i in indices)
            {
                int deckIndex = MyDeckIndices[i];
                
                CardDataSO cardData = gm.masterDeck[deckIndex];
                GameObject previewCard = CreateCard(cardData);

                previewCard.transform.SetParent(drawModal.cardGroupTransform, false);
                previewCard.GetComponent<CardStats>().InitializeCard();
            
                CardDisplay cardDisplay = previewCard.GetComponent<CardDisplay>();
                cardDisplay.FlipCard(true);
                previewCard.GetComponent<CardMovement>().CmdSetCardState(CardMovement.CardState.Preview);
            }
        }

        private List<int> GetRandomUniqueIndices(int count)
        {
            int maxExclusive = MyDeckIndices.Count;

            if (count > maxExclusive)
                throw new ArgumentException("Count cannot be greater than range size");

            List<int> pool = new List<int>();
            for (int i = 0; i < maxExclusive; i++)
                pool.Add(i);

            List<int> result = new List<int>();

            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, pool.Count);
                result.Add(pool[index]);
                pool.RemoveAt(index);
            }

            return result;
        }
    }
}