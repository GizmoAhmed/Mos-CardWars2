using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStatss;
using Extensions;
using UnityEngine;

namespace CardScripts.Abilities.SpellAbilities.DamageAllOppCreatures_Spell
{
    [CreateAssetMenu(fileName = "DamageAllOppCreatures_Spell", menuName = "Abilities/Spell/DamageAllOppCreatures_Spell")]
    public class DamageAllOppCreatures_Spell : CastAbilitySO
    {
        public int damagetoAllAmount;
    
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            List<CreatureStats> oppsCreatures = thisCard.Ext_GetAllOpponentsActiveCreatures();
        
            /*Debug.Log($"<color=yellow>{thisCard.name}</color> damages all opponents " +
                  $"(Counts: {oppsCreatures.Count}) for {damagetoAllAmount}");*/

            foreach (CreatureStats creature in oppsCreatures)
            {
                creature.ChangeCreatureDefense(damagetoAllAmount, false);
            }
        }
    
        public void OnValidate()
        {
            if (castRequirementType != CastRequirementType.AnyTile)
            {
                Debug.LogError($"{name} should have cast type {CastRequirementType.AnyTile}");
            }
        
            if (castSide != CastSide.Theirs)
            {
                Debug.LogError($"{name} should have cast side of {CastSide.Theirs}");
            }
        }
    }
}
