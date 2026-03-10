using AbilityEvents;
using UnityEngine;

namespace CardScripts.Abilities.SpellAbilities.Scripts
{
    [CreateAssetMenu(fileName = "BuffOnCast", menuName = "Abilities/Spell/Buff On Cast")]
    public class BuffOnCast : CardAbilitySO
    {
        public int baseStrengthBuffAmount;
        public int baseDefenseBuffAmount;

        public override bool Condition()
        {
            return true;
        }

        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            Debug.Log($"Executing {name}...");
        }

        public override void OnValidate()
        {
            if (isPassive)
            {
                Debug.LogError($"{name} shouldn't be passive");
            }
        }
    }
}