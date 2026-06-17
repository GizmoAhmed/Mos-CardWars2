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
        GameObject redrawMe = eventData.target;
        
        // check to see if redraw is owned
        bool yours = redrawMe.IsCardOwnedByPlayer(thisCard.GetOwningPlayerStats_Ext());

        if (RollChance(chance)) // hits
        {
            RedrawCard(redrawMe);
            Debug.Log($"<color=cyan>{name}</color> on {thisCard.name} <color=green>activated</color>! ");
        }
        else
        {
            Debug.Log($"<color=grey>{name}</color> on {thisCard.name} <color=red>missed</color>");
        }
    }

    private void OnValidate()
    {
        chance = Mathf.Clamp(chance, 1, 100);
    }
}