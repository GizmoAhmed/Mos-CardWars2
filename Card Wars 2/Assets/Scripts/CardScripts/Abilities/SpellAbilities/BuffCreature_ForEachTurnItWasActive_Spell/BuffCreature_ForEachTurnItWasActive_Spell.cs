using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStatss;
using Extensions;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities.SpellAbilities.BuffCreature_ForEachTurnItWasActive_Spell
{
    [CreateAssetMenu(fileName = "BuffCreature_ForEachTurnItWasActive_Spell",
        menuName = "Abilities/Spell/BuffCreature_ForEachTurnItWasActive_Spell")]
    public class BuffCreature_ForEachTurnItWasActive_Spell : CastAbilitySO
    {
        public int strengthPerTurn;
        
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats creature = eventData.Ext_GetCreatureStats_FromSpellCastEventData();
            CardRuntimeData data = creature.GetComponent<CardRuntimeData>();

            int turnsActive = data.turnsOnField;
            
            int strengthenAmount = turnsActive * strengthPerTurn;
            
            Debug.Log($"<color=yellow>{name} via {thisCard.name}</color> on {creature.gameObject} >>> {strengthPerTurn}(strength per turn) * {turnsActive}(turnsActive) = {strengthenAmount}");
            
            creature.UpdateCreatureStrength(strengthenAmount, buff: true);
        }

        public override bool SpecificSpellPlacementConditions(Tile tile)
        {
            return base.SpecificSpellPlacementConditions(tile);
        }
    }
}
