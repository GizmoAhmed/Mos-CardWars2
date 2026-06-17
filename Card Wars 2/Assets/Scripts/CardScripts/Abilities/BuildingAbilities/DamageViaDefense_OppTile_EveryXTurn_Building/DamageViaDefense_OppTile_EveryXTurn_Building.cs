using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
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
    [Header("Ability Scope")] public int turnsToActivate;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        TurnManager turn = FindObjectOfType<TurnManager>();

        // Debug.Log($"<color=purple>Current turn</color> = {turn._currentTurn}, {thisCard.name} activates every {turnsToActivate} turn");

        if (turnsToActivate < 1)
        {
            // Debug.LogError($"Could not activate {name} on {thisCard.name} because turns to activate on it is set to {turnsToActivate}" );
            return;
        }

        if (turn._currentTurn % turnsToActivate != 0) // this is not turn to activate
        {
            return;
        }

        MiddleTile thisTile = thisCard.GetTile_Ext() as MiddleTile;

        if (thisTile.logicalCreature != null) // no creature
        {
            return;
        }
        
        CreatureStats stats = thisTile.logicalCreature.GetComponent<CreatureStats>();

        int damage = stats.defense;

        if (damage > 0) // sloth could make defense negative
        {
            thisTile.DamageTileAcross_Ext(damage: damage);
        }
        else
        {
            Debug.LogWarning($"{stats.gameObject} must have <color=cyan>sloth</color> on it, because it's defense is below zero and {thisCard.gameObject} can't use that to attack");
        }
    }
}