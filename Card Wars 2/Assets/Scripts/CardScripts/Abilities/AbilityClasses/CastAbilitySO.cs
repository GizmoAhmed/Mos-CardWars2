using AbilityEvents;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities.AbilityClasses
{
    /// <summary>
    /// This for spells, castable cards that can be applied to any middleTile (empty or not)...
    /// ...or placed on a middleTile with a card already on it. 
    /// </summary>
    public abstract class CastAbilitySO : CardAbilitySO
    {
        public enum CastRequirementType
        {
            Free,                   // any middleTile, empty or not, ie deep pockets
            AnywhereOccupied,        // any middleTile that has a card on it
            OnCreature,             // ie buff creature on cast
            OnBuilding,             // ie ...
            OnCharm,                // ie ...
            CreatureAndOrBuilding,  // ie
        }

        public CastRequirementType castRequirementType =  CastRequirementType.Free;

        [Tooltip("True if can place on your side, false if can only place on opponents side, Free cast req ignores this")]
        public bool yourSide; 
        
        public abstract override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);

        // deeper specification for a spell that can't be captured with the above cast requirements. Ie creature needs be a of a certain element or player needs to have x soul left over
        public virtual bool SpecificSpellPlacementConditions(Tile Tile)
        {
            return true; // by default, returning true here just means it has not conditions
        }
    }
}