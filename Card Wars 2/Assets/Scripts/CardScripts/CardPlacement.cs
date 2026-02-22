using Buttons;
using CardScripts.CardData;
using CardScripts.CardDisplays;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using GameManagement;
using Mirror;
using PlayerStuff;
using UnityEngine;

namespace CardScripts
{
    // On player object
    public class CardHandler : NetworkBehaviour
    {
        private GameObject _handGroup1;
        private GameObject _handGroup2;

        private GameObject _discardsBoard1; // your cardboard
        private GameObject _discardsBoard2; // opps card board

        private GameObject _drawModal;

        public void Init()
        {
            _handGroup1 = GameObject.Find("Hand1");
            _handGroup2 = GameObject.Find("Hand2");

            _discardsBoard1 = FindObjectOfType<GameManager>().discardsBoardp1;
            _discardsBoard2 = FindObjectOfType<GameManager>().discardsBoardp2;

            _drawModal = FindObjectOfType<GameManager>().gmVisibleDrawModal;

            if (_handGroup1 == null || _handGroup2 == null)
            {
                Debug.LogError("Group(s) were not set in editor and not found");
            }

            if (_discardsBoard1 == null || _discardsBoard2 == null)
            {
                Debug.LogError("Card board(s) were not found");
                return;
            }

            foreach (DeckButton button in FindObjectsOfType<DeckButton>())
            {
                button.InitDiscardToggle(_discardsBoard1);
            }

            foreach (DrawButton draw in FindObjectsOfType<DrawButton>())
            {
                draw.InitDrawModal(_drawModal);
            }
        }

        [ClientRpc]
        public void MoveCardToHand(GameObject card)
        {
            if (isOwned)
            {
                card.transform.SetParent(_handGroup1.transform, false);
            }
            else
            {
                card.transform.SetParent(_handGroup2.transform, false);

                // hide card in opps hand
                CardDisplay cardDisplay = card.GetComponent<CardDisplay>();
                cardDisplay.FlipCard(false);
            }

            card.GetComponent<CardMovement>().CmdSetCardState(CardMovement.CardState.Hand);
        }

        public void MoveToDiscard(GameObject card)
        {
            if (isOwned)
            {
                card.transform.SetParent(_discardsBoard1.transform.GetChild(0), false);
            }
            else
            {
                card.transform.SetParent(_discardsBoard2.transform.GetChild(0), false);
            }
        }
    }
}