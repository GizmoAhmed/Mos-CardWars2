using AbilityEvents;
using UnityEngine;

namespace CardScripts.Abilities.SpellAbilities.Scripts
{
    [CreateAssetMenu(fileName = "BuffTargetCreatureOnCast", menuName = "Abilities/Spell/BuffTargetCreatureOnCast")]
    public class BuffTargetCreatureOnCast : CastAbilitySO
    {
        public int baseStrengthBuffAmount;
        public int baseDefenseBuffAmount;

        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            Debug.Log($"Executing {name}...");
        }

        public void OnValidate()
        {
            if (castRequirementType != CastRequirementType.OnCreature)
            {
                Debug.LogError($"{name} should have cast type {CastRequirementType.OnCreature}");
            }
        }
    }
}