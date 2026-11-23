using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using UnityEngine;

namespace CardScripts.Abilities
{
    /// <summary>
    /// For creatures with abilities that cost money to use
    /// </summary>
    public abstract class ActiveAbilitySO : CardAbilitySO
    {
        public abstract override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);
        
        /// <summary>
        /// Run this before abilities to see if the ability can actuall be used
        /// </summary>
        /// <param name="thisCard"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public virtual bool ValidateAbility(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats creatureStats = thisCard.GetComponent<CreatureStats>();
            CreatureMovement creatureMovement = thisCard.GetComponent<CreatureMovement>();
            
            if (creatureStats.floopBlockers > 0) return false; // can't floop, abort

            if (creatureMovement.cardState == CardMovement.CardState.Discard)
                return false; // killed itself, abort

            return true;      
        }

        protected virtual void FloatCustomText_FromValidationReject()
        {
            
        }
    }
}