using CardScripts.CardDisplays;
using UnityEngine;

namespace CardScripts
{
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

        public void ClearSavedCard() // kinda redundant but whatever
        {
            currentCard = null;
        }
    }
}
