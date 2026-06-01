using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(fileName = "Redraw_CreatureOnTile_OnDiscard_Building", menuName = "Abilities/Building/Redraw_CreatureOnTile_OnDiscard_Building")]
public class Redraw_CreatureOnTile_OnDiscard_Building : PassiveAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        Debug.Log($"{thisCard.name} calls redraw on {eventData.target.gameObject.name}");
        
        PlayerStats playerStats = thisCard.GetComponent<CardMovement>().thisCardOwnerPlayerStats;
        
        
    }
    
    public void OnValidate()
    {
        if (isGlobalListener && isExecutableOnPlaced && !eventsThatTriggerThisAbility.Contains(AbilityEventType.CardDiscardedFromTile))
        {
            Debug.LogError($"{name} shouldn't be executable on place, and should be a tile listener for CardDiscardedFromTile");
        }
    }
}
