using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities.SpellAbilities.RemoveRunes_OnCreature_Spell
{
    [CreateAssetMenu(fileName = "RemoveRunes_OnCreature_Spell", menuName = "Abilities/Spell/RemoveRunes_OnCreature_Spell")]
    public class RemoveRunes_OnCreature_Spell : CastAbilitySO
    {
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            GameObject creatureOnTile = GetCreatureFromEventData(eventData);
            
            if (creatureOnTile == null)
                return; // error message inside above function
        
            // creature cards have runes slots, that's just how it is
            creatureOnTile.GetComponentInChildren<RuneSlots>().UnbindAllRunes();
        }

        public override bool SpecificSpellPlacementConditions(Tile tile)
        {
            return base.SpecificSpellPlacementConditions(tile);
        }
        
        public void OnValidate()
        {
            if (castRequirementType != CastRequirementType.OnCreature)
            {
                Debug.LogError($"{name} should have cast type {CastRequirementType.OnCreature}");
            }
        
            if (castSide != CastSide.Either)
            {
                Debug.LogError($"{name} should have cast side of {CastSide.Either}");
            }
        }
    }
}
