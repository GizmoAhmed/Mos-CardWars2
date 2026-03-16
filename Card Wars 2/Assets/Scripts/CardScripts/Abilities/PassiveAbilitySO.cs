using System;
using AbilityEvents;
using UnityEngine;

namespace CardScripts.Abilities
{
    /// <summary>
    /// For Buildings, Spells, and Charms. These cards activate automatically when some event happens
    /// </summary>
    public abstract class PassiveAbilitySO : CardAbilitySO
    {
        [Header("Passive Ability Settings")]
        [Tooltip("The events that trigger Execute ability on this SO")]
        public AbilityEventType[] eventsThatTriggerThisAbility;
        
        [Header("Event Scope")]
        [Tooltip("True = Listen globally (events anywhere)\nFalse = Listen locally (events on same tile only)")]
        public bool isGlobalListener = true;
        
        public abstract override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);

        public void OnValidate()
        {
            if (eventsThatTriggerThisAbility == null || eventsThatTriggerThisAbility.Length == 0)
            {
                Debug.LogError($"{name} has no triggering events set in the inspector");
            }
        }
    }
}