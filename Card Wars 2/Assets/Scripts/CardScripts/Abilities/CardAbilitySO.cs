using System;
using AbilityEvents;
using UnityEngine;

namespace CardScripts.Abilities
{
    [Serializable]
    public abstract class CardAbilitySO : ScriptableObject
    {
        [Header("Activation")]
        public bool isPassive = true;
        
        [Header("Passive Ability Settings")]
        [Tooltip("Which events trigger this passive ability? (Only used if isPassive = true)")]
        public AbilityEventType[] triggeringEvents;

        public virtual void OnValidate()
        {
            if (isPassive && (triggeringEvents == null || triggeringEvents.Length == 0))
            {
                Debug.LogError($"{name} is passive but has no triggering events.", this);
            }

            if (!isPassive && triggeringEvents is { Length: > 0 })
            {
                Debug.LogWarning($"{name} has triggering events but is not passive.", this);
            }
        }

        public abstract bool Condition();
        public abstract void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);
    }
}