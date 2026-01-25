using CardScripts.CardDisplays;
using UnityEngine;

public class CardInfoHandler : MonoBehaviour
{
    private CardDisplay current;

    public void HandleCardClick(CardDisplay cardClicked)
    {
        // Case 1: Clicking the currently open card → close it
        if (current == cardClicked)
        {
            current.ToggleInfoSlide(false);
            current = null;
            return;
        }

        // Case 2: Another card is open → close it
        if (current != null)
        {
            current.ToggleInfoSlide(false);
        }

        // Case 3: Open the clicked card
        current = cardClicked;
        current.ToggleInfoSlide(true);
    }
    
    public void CloseAnyOpenInfo()
    {
        if (current == null)
            return;

        current.ToggleInfoSlide(false);
        current = null;
    }

}