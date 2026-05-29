using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using UnityEngine;

[CreateAssetMenu(fileName = "ReduceAbilityCost_OnPlace_Building", menuName = "Abilities/Building/ReduceAbilityCost_OnPlace_Building")]
public class ReduceAbilityCost_OnPlace_Building : PassiveAbilitySO
{
    public int reduction;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureStats = GetCreature_FromTileInEventData(thisCard, eventData);

        if (creatureStats != null)
        {
            // reduce by reduction amount
            creatureStats.ChangeAbilityCost(reduction, increase:false);
        }
    }

    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        // undo what execute does
        CreatureStats creatureStats = GetCreature_FromTileInEventData(thisCard, eventData);

        if (creatureStats != null)
        {
            // reduce by reduction amount
            creatureStats.ChangeAbilityCost(reduction, increase:true);
        }
    }
    
    public void OnValidate()
    {
        if (isGlobalListener && !isExecutableOnPlaced && !eventsThatTriggerThisAbility.Contains(AbilityEventType.CardPlacedOnTile))
        {
            Debug.LogError($"{name} needs to be executable on place and a middleTile listener");
        }
    }
}
