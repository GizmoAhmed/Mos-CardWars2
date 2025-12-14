using System;
using UnityEngine;

namespace Buttons
{
    public class DeckButton : MonoBehaviour
    {
        public GameObject cardBoard;

        public void Init(GameObject c)
        {
            cardBoard = c;
        }

        public void ShowBoard()
        {
            if (cardBoard == null)
            {
                Debug.LogError("Card Field is null, cannot be shown.");
                return;
            }
            
            Debug.Log("Opening Deck...");
            cardBoard.SetActive(!cardBoard.activeInHierarchy); 
        }

    }
}
