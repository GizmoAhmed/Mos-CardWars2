using System;
using UnityEngine;

namespace CardScripts.Abilities
{
    [Serializable]
    public abstract class CardAbilitySO : ScriptableObject
    {
        public abstract bool Condition();
        public abstract void Execute(GameObject thisCard);
    }
}