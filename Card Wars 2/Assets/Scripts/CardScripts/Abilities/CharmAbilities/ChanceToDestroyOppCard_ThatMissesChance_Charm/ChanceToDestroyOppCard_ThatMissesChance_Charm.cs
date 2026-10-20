using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "ChanceToDestroyOppCard_ThatMissesChance_Charm",
    menuName = "Abilities/Charm/ChanceToDestroyOppCard_ThatMissesChance_Charm")]
public class ChanceToDestroyOppCard_ThatMissesChance_Charm : PassiveAbilitySO
{
    [Tooltip("Chance to destroy that card that missed")] [Range(1, 100)]
    public int chanceToHit;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        GameObject cardThatMissed = eventData.target;

        if (cardThatMissed == thisCard) return; // don't trigger on self, below also prevents this but whatever

        bool yours = thisCard.Ext_IsSameOwner(cardThatMissed);

        if (yours) return; // don't care about your own cards

        // there's a situation where two opposing cards of this ability, can loop themselves...
        // ...by continually failing they trigger eachothers ability
        // While this isn't a true inifinte loop (becuase chance cannot be 0, therefore not infinitely triggering, cause it'll hit eventually)...
        // ...it's still important to know
        if (RollChance(chanceToHit, thisCard)) // hits
        {
            CardMovement cardThatMissed_Movement = cardThatMissed.GetComponent<CardMovement>();

            // destroy card that missed
            cardThatMissed_Movement.ServerDiscard();

            AnimateAbilityExecute(thisCard);
        }
    }
}