using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using PlayerStuff;
using Tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "EarnShardsEachTurn_IfNoCreatureOnTile_Building", menuName = "Abilities/Building/EarnShardsEachTurn_IfNoCreatureOnTile_Building")]
public class EarnShardsEachTurn_IfNoCreatureOnTile_Building : PassiveAbilitySO
{
    public int shardsEarned;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        Debug.Log("Contemplating giving shards...");
        
        BuildingMovement move = thisCard.GetComponent<BuildingMovement>();

        MiddleTile tile = move.GetLogicalTile() as MiddleTile;

        if (tile?.logicalCreature == null) // tile shouldn't be null but whatever
        {
            PlayerStats player = thisCard.GetComponent<BuildingMovement>().thisCardOwnerPlayerStats;
            player.shards += shardsEarned;
            
            Debug.Log($"Giving {player} {shardsEarned} shards from {thisCard.name}");
        }
        else
        {
            Debug.LogWarning($"Can't give shards because {tile.logicalCreature} is sharing tile with {thisCard.name}");
        }
    }

    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        // Debug.Log("Does nothing....");
    }

    public void OnValidate()
    {
        if (!isGlobalListener && isExecutableOnPlaced)
        {
            Debug.LogError($"{name} listen globally and be not executable on place");
        }
    }
}
