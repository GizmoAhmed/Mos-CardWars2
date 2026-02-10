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
        // acts exactly like sync var, changes made to this on server reflect on each client
        public SyncList<GameObject> MyDeck =  new SyncList<GameObject>();

        [Command]
        public void CmdDrawCard()
        {
            DrawCardFromDeck(connectionToClient);
        }

        private void DrawCardFromDeck(NetworkConnectionToClient conn)
        {
            if (MyDeck.Count == 0)
            {
                Debug.LogWarning($"Empty Deck, Player {connectionToClient.connectionId + 1} Can't DrawButton");
                return;
            }
            
            int randomIndex = 0;
            GameObject cardInstance = MyDeck[randomIndex];

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

            MyDeck.RemoveAt(randomIndex);
        }
        
        [Server]
        public void PreviewOfferedCards(NetworkConnectionToClient target, int offer)
        {
            List<int> randomIndices = GetRandomUniqueIndices(offer);

            TargetShowCardPreviews(target, randomIndices.ToArray());
        }

        private List<int> GetRandomUniqueIndices(int count)
        {
            int maxExclusive = MyDeck.Count;

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
                GameObject prefab = deck.MyDeck[index];

                GameObject previewCard = Instantiate(prefab, modal.cardGroupTransform, false);
            }
        }
    }
}