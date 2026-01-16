using Buttons;
using Mirror;
using UnityEngine;

namespace CardScripts
{
    // On player object
    public class CardHandler : NetworkBehaviour
    {
        private GameObject _playingCardGroup1;
        private GameObject _playingCardGroup2;

        private GameObject cardBoard1; // your cardboard
        private GameObject cardBoard2; // opps card board

        public void Init()
        {
            _playingCardGroup1 = GameObject.Find("PlayingCardGroup1");
            _playingCardGroup2 = GameObject.Find("PlayingCardGroup2");

            cardBoard1 = FindObjectOfType<GameManager>().cardBoard1;
            cardBoard2 = FindObjectOfType<GameManager>().cardBoard2;

            if (_playingCardGroup1 == null || _playingCardGroup2 == null)
            {
                Debug.LogError("Group(s) were not set in editor and not found");
            }

            if (cardBoard1 == null || cardBoard2 == null)
            {
                Debug.LogError("Card board(s) were not found");
                return;
            }

            foreach (DeckButton button in FindObjectsOfType<DeckButton>())
            {
                button.Init(cardBoard1);
            }
        }

        [Command]
        public void CmdDropCard(GameObject card, GameObject land)
        {
            PlaceCardOnLand(card, land);
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

            card.GetComponent<CardMovement>().cardState = CardMovement.CardState.Hand;
        }

        /// <summary>
        /// Handles cards as they are being spawned or placed
        ///
        /// Cards can be:
        /// - Spawned from the deck list and put in hand
        /// - Placing them on the field 
        /// </summary>
        /// <param name="card"></param>
        /// <param name="land"></param>
        [ClientRpc]
        public void PlaceCardOnLand(GameObject card, GameObject land)
        {
            // if (spell) {don't attach, just activate discard}.
            // or todo Rune
            if (card.GetComponent<CardStats>().cardData.cardType == CardDataSO.CardType.Spell)
            {
                card.GetComponent<CardMovement>().Discard(); // ...where they are immediately discarded upon cast

                Debug.Log(card.name + " was cast on " + land.name);

                return; // else if (passive) {just continue since it'll treat it like a creature or building, but in the spell area.}
            }

            // do same for Runes

            // CardMovement.cs already makes sure card can actually be placed on this land via ValidPlace
            if (isOwned)
            {
                land.GetComponent<MiddleLand>().AttachCard(card);
            }
            else
            {
                MiddleLand landScript = land.GetComponent<MiddleLand>();
                MiddleLand acrossLand = landScript.across.GetComponent<MiddleLand>();

                acrossLand.AttachCard(card);
            }

            card.GetComponent<CardMovement>().cardState = CardMovement.CardState.Field;
        }

        public void MoveToDiscard(GameObject card)
        {
            // .GetChild(0) should be DiscardVisual, as it's the first child

            if (isOwned)
            {
                card.transform.SetParent(cardBoard1.transform.GetChild(0), false);
            }
            else
            {
                card.transform.SetParent(cardBoard2.transform.GetChild(0), false);
            }
        }
    }
}