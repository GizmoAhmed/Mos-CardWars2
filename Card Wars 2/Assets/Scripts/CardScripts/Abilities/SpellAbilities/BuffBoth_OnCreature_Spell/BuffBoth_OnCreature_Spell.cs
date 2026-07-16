using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStatss;
using Extensions;
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
            CreatureStats creatureStats = eventData.Ext_GetCreatureStats_FromSpellCastEventData();
            
            if (creatureStats == null)
                return; // error message inside above function
            
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