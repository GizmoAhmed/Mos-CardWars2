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
            AdjustCost(card: thisCard, costIncrease, increase: true); // increase all
            // AnimateAbilityExecute(thisCard);
        }
        else if (eventData.eventType == AbilityEventType.AnyCreaturePlaced)
        {
            bool owned = thisCard.Ext_IsSameOwner(eventData.target);   

            if (!owned) // make other creatures card
            {
                CreatureStats placedCreature = eventData.target.GetComponent<CreatureStats>();

                // placedCreature.abilityCost += costIncrease;
                
                // increase the creature just placed
                placedCreature.UpdateAbilityCost(amount: costIncrease, increase: true, source: thisCard.transform);
                AnimateAbilityExecute(thisCard);
            }
        }
    }
    
    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        AdjustCost(card: thisCard, costIncrease, increase: false); // we outta here, so reduce burn cost back
    }
    
    private void AdjustCost(GameObject card, int amount, bool increase)
    {
        var creatures = card.Ext_GetAllOpponentsActiveCreatures();

        foreach (var creature in creatures)
        {
            CreatureStats stats = creature.GetComponent<CreatureStats>();

            if (increase) // increase all
            {
                stats.UpdateAbilityCost(amount: costIncrease, increase: true, source: card.transform);
            }
            else // reduce all
            {
                stats.UpdateAbilityCost(amount: costIncrease, increase: false, source: card.transform);
            }
        }
    }
}