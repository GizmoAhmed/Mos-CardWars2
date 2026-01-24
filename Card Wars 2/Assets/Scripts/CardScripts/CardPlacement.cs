using Buttons;
using CardScripts.CardData;
using CardScripts.CardDisplays;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Lands;
using Mirror;
using UnityEngine;

namespace CardScripts
{
    // On player object
    public class CardHandler : NetworkBehaviour
    {
        private GameObject _playingCardGroup1;
        private GameObject _playingCardGroup2;

        private GameObject _cardBoard1; // your cardboard
        private GameObject _cardBoard2; // opps card board

        public void Init()
        {
            _playingCardGroup1 = GameObject.Find("PlayingCardGroup1");
            _playingCardGroup2 = GameObject.Find("PlayingCardGroup2");

            _cardBoard1 = FindObjectOfType<GameManager>().cardBoard1;
            _cardBoard2 = FindObjectOfType<GameManager>().cardBoard2;

            if (_playingCardGroup1 == null || _playingCardGroup2 == null)
            {
                Debug.LogError("Group(s) were not set in editor and not found");
            }

            if (_cardBoard1 == null || _cardBoard2 == null)
            {
                Debug.LogError("Card board(s) were not found");
                return;
            }

            foreach (DeckButton button in FindObjectsOfType<DeckButton>())
            {
                button.Init(_cardBoard1);
            }
        }

        [ClientRpc]
        public void MoveCardToHand(GameObject card)
        {
            if (isOwned)
            {
                card.transform.SetParent(_playingCardGroup1.transform, false);
            }
            else
            {
                card.transform.SetParent(_playingCardGroup2.transform, false);

                // hide card in opps hand
                CardDisplay cardDisplay = card.GetComponent<CardDisplay>();
                cardDisplay.FlipCard(false);
            }

            card.GetComponent<BaseMovement>().cardState = BaseMovement.CardState.Hand;
        }

        public void MoveToDiscard(GameObject card)
        {
            if (isOwned)
            {
                card.transform.SetParent(_cardBoard1.transform.GetChild(0), false);
            }
            else
            {
                card.transform.SetParent(_cardBoard2.transform.GetChild(0), false);
            }
        }
    }
}