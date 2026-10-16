using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;

[CreateAssetMenu(
    fileName = "IncreaseAllOppCreaturesAbilityCost_Charm", 
    menuName = "Abilities/Charm/IncreaseAllOppCreaturesAbilityCost_Charm")]
public class IncreaseAllOppCreaturesAbilityCost_Charm : PassiveAbilitySO
{
    public int costIncrease;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (eventData.eventType == AbilityEventType.CardPlacedOnTile)
        {
            AdjustCost(card: thisCard, -costIncrease);
            // AnimateAbilityExecute(thisCard);
        }
        else if (eventData.eventType == AbilityEventType.AnyCreaturePlaced)
        {
            bool owned = thisCard.Ext_IsSameOwner(eventData.target);   

            if (!owned) // make other creatures card
            {
                CreatureStats placedCreature = eventData.target.GetComponent<CreatureStats>();

                placedCreature.abilityCost += costIncrease;
                AnimateAbilityExecute(thisCard);
            }
        }
    }
    
    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        AdjustCost(card: thisCard, costIncrease); // negative, since increasing burn cost
    }
    
    private void AdjustCost(GameObject card, int amount)
    {
        var creatures = card.Ext_GetAllOpponentsActiveCreatures();

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