using System;
using UnityEngine;

namespace Buttons
{
    public class DeckButton : MonoBehaviour
    {
        public GameObject discardBoard;

        public void InitDiscardToggle(GameObject c)
        {
            discardBoard = c;
        }

        public void ShowBoard()
        {
            if (discardBoard == null)
            {
                Debug.LogError("Card Field is null, cannot be shown.");
                return;
            }
            
            Debug.Log("Opening Deck...");
            discardBoard.SetActive(!discardBoard.activeInHierarchy); 
        }
    }
}
