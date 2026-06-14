using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
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

        Debug.Log($"<color=purple>Current turn</color> = {turn._currentTurn}, {thisCard.name} activates every {turnsToActivate} turn");

        if (turnsToActivate < 1)
        {
            Debug.LogError($"Could not activate {name} on {thisCard.name} because turns to activate on it is set to {turnsToActivate}" );
            return;
        }

        if (turn._currentTurn % turnsToActivate != 0) // this is not turn to activate
        {
            return;
        }

        BuildingMovement move = thisCard.GetComponent<BuildingMovement>();

        MiddleTile thisTile = move.GetLogicalTile() as MiddleTile;

        if (thisTile.logicalCreature == null) // no creature
        {
            Debug.Log($"<color=brown>{name}</color> has no logical creature");
        }
        else
        {
            CreatureStats stats = thisTile.logicalCreature.GetComponent<CreatureStats>();

            int damage = stats.defense;

            if (damage > 0) // sloth could make defense negative
            {
                MiddleTile across = TileManager.Instance.GetAcrossTile(thisTile.row,
                        thisTile.column,
                        thisTile.serverPlayerSide)
                    as MiddleTile;

                if (across.logicalCreature != null)
                {
                    CreatureStats oppCreature = across.logicalCreature.GetComponent<CreatureStats>();

                    Debug.Log($"<color=brown>{thisCard.name}</color>: Damaging {oppCreature} for {damage}");
                    
                    // deal damage to opposing
                    oppCreature.ChangeCreatureDefense(damage, buff: false);
                }
                else
                {
                    Debug.Log($"<color=brown>{thisCard.name}</color>: Damaging empty tile for {damage}");

                    // todo damage to empty lane
                    // just pass damage to tiles script, and they can handle it, so we don't right this if block everytime
                }
            }
        }
    }
}