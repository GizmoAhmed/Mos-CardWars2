using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStatss;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities.SpellAbilities.Scripts
{
    [CreateAssetMenu(fileName = "BasicTargetDamage", menuName = "Abilities/Spell/BasicTargetDamage")]
    public class BasicTargetDamage : CastAbilitySO
    {
        public int damage;

        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            MiddleTile middleTile = eventData.CardToBeAffected.GetComponent<MiddleTile>();
            GameObject creatureOnTile = middleTile.logicalCreature;
            
            if (creatureOnTile == null)
            {
                Debug.LogError($"{name} could not find creature on middleTile {middleTile.gameObject.name}");
                return;
            }
            
            CreatureStats creatureStats = creatureOnTile.GetComponent<CreatureStats>();
            
            // deal damage
            creatureStats.ChangeCreatureDefense(damage, buff: false);
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
