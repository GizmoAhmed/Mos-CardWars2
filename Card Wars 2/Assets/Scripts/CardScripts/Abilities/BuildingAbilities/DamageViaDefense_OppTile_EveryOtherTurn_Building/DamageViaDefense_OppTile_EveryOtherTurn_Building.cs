using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using GameManagement;
using Tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageViaDefense_OppTile_EveryOtherTurn_Building",
    menuName = "Abilities/Building/DamageViaDefense_OppTile_EveryOtherTurn_Building")]
public class DamageViaDefense_OppTile_EveryOtherTurn_Building : PassiveAbilitySO
{
    [Header("Ability Scope")] public int turnsToActivate;

    [SerializeField] private int currentTurn = 0;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // Debug.Log($"{name} is activating...");

        if (currentTurn < turnsToActivate)
        {
            Debug.LogWarning($"<color=brown>{name}</color> will activate in {turnsToActivate - currentTurn} more turn");
            currentTurn++;
            return;
        }

        BuildingMovement move = thisCard.GetComponent<BuildingMovement>();

        MiddleTile thisTile = move.GetLogicalTile() as MiddleTile;

        if (thisTile.logicalCreature == null) // no creature
        {
            // Debug.Log($"<color=brown>{name}</color> has no logical creature");
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

                    // Debug.Log($"<color=brown>{name}</color>: Damaging {oppCreature} for {damage}");
                    
                    // deal damage to oppsoing
                    oppCreature.ChangeCreatureDefense(damage, buff: false);
                }
                else
                {
                    // todo damage to empty lane
                    // just pass damage to tiles script, and they can handle it, so we don't right this if block everytime
                }
            }
        }

        currentTurn = 0; // reset back
    }

    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        currentTurn = 0;
    }
}