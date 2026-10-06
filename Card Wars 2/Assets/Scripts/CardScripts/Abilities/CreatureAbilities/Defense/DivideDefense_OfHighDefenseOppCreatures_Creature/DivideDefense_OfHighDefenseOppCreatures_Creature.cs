using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DivideDefense_OfHighDefenseOppCreatures_Creature", 
    menuName = "Abilities/Creature/DivideDefense_OfHighDefenseOppCreatures_Creature")]
public class DivideDefense_OfHighDefenseOppCreatures_Creature : ActiveAbilitySO
{
    [Tooltip("Opp creature that have defense <= to this, get effected")]
    public int defenseThreshold;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        List<CreatureStats> oppFieldCreatures = thisCard.Ext_GetAllOpponentsActiveCreatures();

        foreach (CreatureStats oppFieldCard in oppFieldCreatures)
        {
            if (oppFieldCard.defense >= defenseThreshold)
            {
                int debuffAmount = oppFieldCard.defense / 2; // half
                
                //debuff
                oppFieldCard.UpdateCreatureDefense(amount: debuffAmount, buff: false, applyMult: true);
            }
        }
        
        AnimateAbilityExecute(thisCard);
    }
}