using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Extensions;
using GameManagement;
using Tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageViaDefense_OppTile_EveryXTurn_Building",
    menuName = "Abilities/Building/DamageViaDefense_OppTile_EveryXTurn_Building")]
public class DamageViaDefense_OppTile_EveryXTurn_Building : PassiveAbilitySO
{
    public int turnsToActivate;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (turnsToActivate < 1)
        {
            Debug.LogError($"Could not activate {name} on {thisCard.name} because turns to activate on it is set to {turnsToActivate}" );
            return;
        }
        
        CardRuntimeData cardRuntimeData = thisCard.GetComponent<CardRuntimeData>();
        int turnsActive = cardRuntimeData.turnsOnField;

        if (turnsActive % turnsToActivate != 0) // this is not turn to activate
        {
            return;
        }

        MiddleTile thisTile = thisCard.Ext_GetTile() as MiddleTile;

        if (thisTile == null || thisTile.logicalCreature == null) // no creature
        {
            return;
        }
        
        CreatureStats stats = thisTile.logicalCreature.GetComponent<CreatureStats>();

        int damage = stats.defense;

        if (damage > 0) // sloth could make defense negative
        {
            thisTile.DamageTileAcross_Ext(damage: damage);
            AnimateAbility(thisCard);
        }
        else
        {
            Debug.LogWarning($"Strength on {stats.gameObject} is below zero, must have <color=cyan>sloth</color> on it. {name} can't use a negative defense value to attack");
        }
    }
}