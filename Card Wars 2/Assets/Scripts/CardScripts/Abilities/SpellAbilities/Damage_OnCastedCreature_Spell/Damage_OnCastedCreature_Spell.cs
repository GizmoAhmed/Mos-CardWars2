using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStatss;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities.SpellAbilities.Scripts
{
    [CreateAssetMenu(fileName = "Damage_OnCastedCreature_Spell", menuName = "Abilities/Spell/Damage_OnCastedCreature_Spell")]
    public class Damage_OnCastedCreature_Spell : CastAbilitySO
    {
        public int damage;

        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            GameObject creatureOnTile = GetCreatureFromEventData(eventData);
            
            if (creatureOnTile == null)
                return; // error message inside above function
            
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
