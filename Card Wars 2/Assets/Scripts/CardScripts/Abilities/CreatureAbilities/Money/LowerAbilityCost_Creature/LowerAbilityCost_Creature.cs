using AbilityEvents;
using CardScripts.CardStatss;
using UnityEngine;

namespace CardScripts.Abilities.CreatureAbilities.Money.LowerAbilityCost_Creature
{
    [CreateAssetMenu(fileName = "LowerAbilityCost_Creature",
        menuName = "Abilities/Creature/Money/LowerAbilityCost_Creature")]
    public class LowerAbilityCost_Creature : ActiveAbilitySO
    {
        public int reduction;

        [Tooltip("This is the lowest amount that this ability will reduce itself to." +
                 "\nIf it's not set or to low, players can get a LOT of money")]
        public int lowestCostReduction;

        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            if (lowestCostReduction > 0) // if true, ability really useless
            {
                Debug.LogError($"<color=yellow>{name}</color> on {thisCard.name} doesn't make since ability cost threshold " +
                               $"should probably be negative, it's currently set to: {lowestCostReduction}");

                return;
            }

            CreatureStats creatureStats = thisCard.GetComponent<CreatureStats>();
            
            if (creatureStats.abilityCost > lowestCostReduction) // for balance, don't let ability cost go to low that would give the player too much money
            {
                // player experience: realize ability cost can go negative, eventually giving money
                creatureStats.ChangeAbilityCost(amount: reduction, increase: false);
                AnimateAbility(thisCard);
            }
        }
    }
}