using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ReduceAllYourCreaturesAbilityCost_Charm", 
    menuName = "Abilities/Charm/ReduceAllYourCreaturesAbilityCost_Charm")]
public class ReduceAllYourCreaturesAbilityCost_Charm : PassiveAbilitySO
{
    public int costReduction;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (eventData.eventType == AbilityEventType.CardPlacedOnTile)
        {
            AdjustCost(card: thisCard, costReduction);
            // AnimateAbilityExecute(thisCard);
        }
        else if (eventData.eventType == AbilityEventType.AnyCreaturePlaced)
        {
            bool owned = thisCard.Ext_IsSameOwner(eventData.target);   

            if (owned)
            {
                CreatureStats placedCreature = eventData.target.GetComponent<CreatureStats>();

                placedCreature.abilityCost -= costReduction;
                AnimateAbilityExecute(thisCard);
            }
        }
    }
    
    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        AdjustCost(card: thisCard, -costReduction); // negative, since increasing burn cost
    }
    
    private void AdjustCost(GameObject card, int amount)
    {
        var creatures = card.Ext_GetAllActiveCreaturesForThisPlayer();

        foreach (var creature in creatures)
        {
            CreatureStats stats = creature.GetComponent<CreatureStats>();

            if (amount > 0) // reducing
            {
                stats.abilityCost -= amount;
            }
            else // adding back from undo
            {
                stats.abilityCost -= amount;
            }
        }
    }
}