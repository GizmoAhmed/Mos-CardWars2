using System;
using AbilityEvents;
using UnityEngine;

namespace CardScripts.Abilities
{
    /// <summary>
    /// This for spells, castable cards that can be applied to any tile (empty or not)...
    /// ...or placed on a tile with a card already on it. 
    /// </summary>
    public abstract class CastAbilitySO : CardAbilitySO
    {
        public enum CastRequirementType
        {
            Free,                   // any tile, empty or not, ie deep pockets
            AnywhereOccupied,        // any tile that has a card on it
            OnCreature,             // ie buff creature on cast
            OnBuilding,             // ie ...
            OnCharm,                // ie ...
            CreatureAndOrBuilding,  // ie
        }

        public CastRequirementType castRequirementType =  CastRequirementType.Free;

        [Tooltip("True if can place on your side, false if can only place on opponents side, Free cast req ignores this")]
        public bool yourSide; 
        
        public abstract override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);
    }
}