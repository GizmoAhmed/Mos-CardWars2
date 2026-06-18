using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(fileName = "RedrawCardOnChance_OnDraw_Charm",
    menuName = "Abilities/Charm/RedrawCardOnChance_OnDraw_Charm")]
public class RedrawCardOnChance_OnDraw_Charm : PassiveAbilitySO
{
    [Header("Chance to Hit")]
    [Range(1, 100)] public int chance;

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
        bool yours = redrawMe.IsCardOwnedByPlayer(thisCard.GetOwningPlayerStats_Ext());

        if (!yours) return;
        
        if (RollChance(chance)) // hits
        {
            // this ability listens for draws and draws itself, so...
            // to prevent loops, pass boolean to break the cycle for this specific ability
            RedrawCard(redrawMe, isDuplicate: true); 
            
            Debug.Log($"<color=cyan>{name}</color> on {thisCard.name} <color=green>activated</color>! ");
        }
        else // misses
        {
            Debug.Log($"<color=grey>{name}</color> on {thisCard.name} <color=red>missed</color>");
        }
    }

    private void OnValidate()
    {
        chance = Mathf.Clamp(chance, 1, 100);
    }
}