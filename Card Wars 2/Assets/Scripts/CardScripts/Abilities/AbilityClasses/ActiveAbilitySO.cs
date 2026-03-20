using AbilityEvents;
using UnityEngine;

namespace CardScripts.Abilities
{
    /// <summary>
    /// For creatures with abilities that cost money to use
    /// </summary>
    public abstract class ActiveAbilitySO : CardAbilitySO
    {
        public abstract override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);
    }
}