using System.Collections.Generic;
using CardScripts;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Mirror;
using UnityEngine;

namespace PlayerStuff
{
    public class DeckCollection : NetworkBehaviour
    {
        // currently only server player gets to see updated lists
        public List<GameObject> myDeck;

        [Command]
        public void CmdDrawCard()
        {
            DrawCardFromDeck(connectionToClient);
        }

        private void DrawCardFromDeck(NetworkConnectionToClient conn)
        {
            if (myDeck.Count == 0)
            {
                Debug.LogWarning($"Empty Deck, Player {connectionToClient.connectionId + 1} Can't DrawButton");
                return;
            }
            
            int randomIndex = 0;
            GameObject cardInstance = myDeck[randomIndex];

            // add to scene
            GameObject drawnCard = Instantiate(cardInstance);

            // set owner to player who drew it
            BaseMovement move = drawnCard.GetComponent<BaseMovement>();
            PlayerStats stats = GetComponentInParent<PlayerStats>();
            move.SetOwningPlayer(stats);

            // add it to the server for both players
            NetworkServer.Spawn(drawnCard, conn);

            Player player = GetComponentInParent<Player>();
            
            if (player == null)
            {
                Debug.LogError("Player is null, can't draw");
                return;
            }
            
            player.cardPlacer.MoveCardToHand(drawnCard);

            myDeck.RemoveAt(randomIndex);
        }
        
        [Server]
        public void PreviewOfferedCards(NetworkConnectionToClient target, int offer)
        {
            List<int> randomIndices = GetRandomUniqueIndices(offer);

            TargetShowCardPreviews(target, randomIndices.ToArray());
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
        
        [TargetRpc]
        private void TargetShowCardPreviews(NetworkConnection target, int[] cardIndices)
        {
            DrawModal modal = FindObjectOfType<DrawModal>();
            DeckCollection deck = GetComponent<DeckCollection>();

            foreach (int index in cardIndices)
            {
                GameObject prefab = deck.myDeck[index];

                GameObject previewCard = Instantiate(prefab, modal.cardGroupTransform, false);
            }
        }
    }
}