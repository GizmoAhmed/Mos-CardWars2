using System.Collections;
using System.Collections.Generic;
using CardScripts;
using UnityEngine;

public class CardInfoHandler : MonoBehaviour
{
    public CardDisplay currentCard;
    
    void Start()
    {
        currentCard = null;
    }

    public void SaveInfo(GameObject card)
    {
        if (currentCard != null)
        {
            currentCard.ToggleInfoSlide(false); // turn off the one already in there
        }

        currentCard = card.GetComponent<CardDisplay>();   
    }

    public void ClearSavedCard()
    {
        currentCard = null;
    }
}
