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
            creatureStats.UpdateAbilityCost(reduction, increase:false, source: thisCard.transform);
            AnimateAbilityExecute(thisCard);
        }
    }

    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        // undo what execute does
        CreatureStats creatureStats = GetCreature_FromTileInEventData(thisCard, eventData);

        if (creatureStats != null)
        {
            // increase back to original
            creatureStats.UpdateAbilityCost(reduction, increase:true, source: thisCard.transform);
        }
    }
}
