using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStatss;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities.SpellAbilities.BuffBoth_OnCreature_Spell
{
    [CreateAssetMenu(fileName = "BuffBoth_OnCreature_Spell", menuName = "Abilities/Spell/BuffBoth_OnCreature_Spell")]
    public class BuffBoth_OnCreature_Spell : CastAbilitySO
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