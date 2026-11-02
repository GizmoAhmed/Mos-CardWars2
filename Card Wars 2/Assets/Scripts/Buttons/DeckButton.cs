using System;
using CardScripts;
using Modal;
using UnityEngine;

namespace Buttons
{
    public class DeckButton : MonoBehaviour
    {
        public GameObject discardBoard;
        
        private CardInfoHandler _cardInfoHandler;
        
        private DrawModal _drawModal;

        public void InitDiscardToggle(GameObject c)
        {
            discardBoard = c;
            
            _cardInfoHandler = FindObjectOfType<CardInfoHandler>();
            
            _drawModal = FindAnyObjectByType<DrawModal>(FindObjectsInactive.Include);
        }

        public void ShowBoard()
        {
            if (discardBoard == null)
            {
                Debug.LogError("Card Field is null, cannot be shown.");
                return;
            }
            
            // Debug.Log("Opening Deck...");
            discardBoard.SetActive(!discardBoard.activeInHierarchy); 
            
            _cardInfoHandler.CloseAnyOpenInfo();

            if (_drawModal.gameObject.activeInHierarchy) // if the draw modal is open while opening this one...
            {
                _drawModal.MinimizeModal();  // ...minimize it
            }
        }
    }
}
