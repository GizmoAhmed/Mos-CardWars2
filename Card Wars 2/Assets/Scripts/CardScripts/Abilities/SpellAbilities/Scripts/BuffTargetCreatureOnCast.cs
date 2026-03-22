using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStatss;
using Tiles;
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
            MiddleTile middleTile = eventData.CardToBeAffected.GetComponent<MiddleTile>();
            GameObject creatureOnTile = middleTile.logicalCreature;

            if (creatureOnTile == null)
            {
                Debug.LogError($"Could not find creature on middleTile {middleTile.gameObject.name}");
                return;
            }
            
            CreatureStats creatureStats = creatureOnTile.GetComponent<CreatureStats>();
            
            creatureStats.ChangeCreatureStrength(baseStrengthBuffAmount, buff: true);
            creatureStats.ChangeCreatureDefense(baseDefenseBuffAmount, buff: true);
            
            // Debug.LogWarning($"{name} on {thisCard.name} buffs {creatureStats.gameObject.name}: + {baseStrengthBuffAmount} strength and + {baseDefenseBuffAmount} defense");
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