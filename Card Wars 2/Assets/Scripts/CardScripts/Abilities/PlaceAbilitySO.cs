using AbilityEvents;
using UnityEngine;

namespace CardScripts.Abilities
{
    // place ability is a passive ability that only listens to card placement and casts like a spell.
    //  listening and casting result in the same execution.
    // When this card is discard, undo its execution on the affected card
    public abstract class PlaceAbilitySO : PassiveAbilitySO
    {
        void PlaceAbility()
        {
            isGlobalListener = false;
            eventsThatTriggerThisAbility = new AbilityEventType[] { AbilityEventType.CardPlacedOnTile };
        }

        public abstract override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);
        
        public abstract void UndoExecution(GameObject thisCard, AbilityEventData eventData);
        
        
    }
}