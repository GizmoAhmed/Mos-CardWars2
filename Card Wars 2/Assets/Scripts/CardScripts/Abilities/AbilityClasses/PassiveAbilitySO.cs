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
        [Tooltip("True = Listen globally (events anywhere)\nFalse = Listen locally (events on same middleTile only)")]
        public bool isGlobalListener = true;

        [Tooltip("Executes its ability on place (like a spell), along with passively listening")]
        public bool isExecutableOnPlaced = false;
        
        public abstract override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);

        public virtual void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            Debug.LogWarning($"Called base.UndoExecution on {name} when should be using child override");
        }
    }
}