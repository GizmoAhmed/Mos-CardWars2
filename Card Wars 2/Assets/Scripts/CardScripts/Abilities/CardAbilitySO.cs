using System;
using AbilityEvents;
using UnityEngine;

namespace CardScripts.Abilities
{
    [Serializable]
    public abstract class CardAbilitySO : ScriptableObject
    {
        public abstract void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);
    }
}