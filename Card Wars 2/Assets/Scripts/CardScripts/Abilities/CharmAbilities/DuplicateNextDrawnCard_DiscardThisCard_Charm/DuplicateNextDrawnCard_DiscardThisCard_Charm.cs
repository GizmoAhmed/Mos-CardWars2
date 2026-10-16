using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DuplicateNextDrawnCard_DiscardThisCard_Charm", 
    menuName = "Abilities/Charm/DuplicateNextDrawnCard_DiscardThisCard_Charm")]
public class DuplicateNextDrawnCard_DiscardThisCard_Charm : PassiveAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // true or false, passed from redraw below
        // adds flag to event data to tell event system
        // that a redrawn card from this ability is a duplicate
        // so that it doesn't trigger itself over and over
        object dupe = eventData.CustomData["isDuplicate"];

        if ((bool)dupe) return; // attempt cast as bool (dw, should always be bool)
        
        GameObject redrawMe = eventData.target;
        
        // check to see if redraw is owned
        bool yours = redrawMe.Ext_IsSameOwner(thisCard);
        
        if (!yours) return;
        
        // draw that card again
        RedrawCard(redrawMe, isDuplicate: true);
        
        // discard this one
        thisCard.GetComponent<CharmMovement>().ServerDiscard();
    }
}