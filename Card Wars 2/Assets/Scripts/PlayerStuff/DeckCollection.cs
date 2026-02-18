using System.Collections.Generic;
using System.Reflection;
using CardScripts;
using CardScripts.CardData;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using GameManagement;
using Mirror;
using UnityEngine;

namespace PlayerStuff
{
    public class DeckCollection : NetworkBehaviour
    {
        public List<CardDataSO> myDeck;

        [Command]
        public void CmdDrawCard()
        {
            DrawCardFromDeck(connectionToClient);
        }

        private void DrawCardFromDeck(NetworkConnectionToClient conn)
        {
            if (myDeck.Count == 0)
            {
                Debug.LogWarning($"Empty Deck, Player {connectionToClient.connectionId + 1} Can't draw");
                return;
            }
            
            int randomIndex = 0;
            CardDataSO drawnCardData = myDeck[randomIndex];

            // add to scene
            // GameObject drawnCard = Instantiate(drawnCardData);
            GameObject cardObj = CreateCard(drawnCardData);

            // set owner to player who drew it
            /*CardMovement move = drawnCard.GetComponent<CardMovement>();
            PlayerStats stats = GetComponentInParent<PlayerStats>();
            move.SetOwningPlayer(stats);*/

            // add it to the server for both players
            NetworkServer.Spawn(cardObj, conn);

            Player player = GetComponentInParent<Player>();
            
            if (player == null)
            {
                Debug.LogError("Player is null, can't draw");
                return;
            }
            
            player.cardPlacer.MoveCardToHand(cardObj);

            myDeck.RemoveAt(randomIndex);
        }

        private GameObject CreateCard(CardDataSO data)
        {
            GameObject card;
            
            GameManager gm = FindObjectOfType<GameManager>();

            if      (data is CreatureDataSO) card = gm.creatureCard;
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
            
            cardInstance.GetComponent<CardStats>().InitializeCard(data);
            
            // set owner to player who drew it
            CardMovement move = cardInstance.GetComponent<CardMovement>();
            PlayerStats stats = GetComponentInParent<PlayerStats>();
            move.SetOwningPlayer(stats);
            
            return cardInstance;
        }

        [Server]
        public void PreviewOfferedCards(NetworkConnectionToClient target, int offer)
        {
            List<int> randomIndices = GetRandomUniqueIndices(offer);

            // TargetShowCardPreviews(target, randomIndices.ToArray());
        }

        private List<int> GetRandomUniqueIndices(int count)
        {
            int maxExclusive = myDeck.Count;

            if (count > maxExclusive)
                throw new System.ArgumentException("Count cannot be greater than range size");

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
        
        /*[TargetRpc]
        private void TargetShowCardPreviews(NetworkConnection target, int[] cardIndices)
        {
            DrawModal modal = FindObjectOfType<DrawModal>();
            DeckCollection deck = GetComponent<DeckCollection>();

            foreach (int index in cardIndices)
            {
                GameObject prefab = deck.myDeck[index];

                GameObject previewCard = Instantiate(prefab, modal.cardGroupTransform, false);
                
                CardMovement move = previewCard.GetComponent<CardMovement>();
                PlayerStats stats = GetComponentInParent<PlayerStats>();
                move.SetOwningPlayer(stats);
            }
        }*/
    }
}