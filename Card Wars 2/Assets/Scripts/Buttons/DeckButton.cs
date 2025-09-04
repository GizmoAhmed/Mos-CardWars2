using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckButton : MonoBehaviour
{
    public GameObject cardBoard;

    public void ShowBoard()
    {
        if (cardBoard == null)
        {
            Debug.LogError("Card Board is not assigned and could not be found.");
            return;
        }

        Debug.Log("Opening Deck...");
        cardBoard.SetActive(!cardBoard.activeInHierarchy); 
    }

}
