using System;
using System.Linq;
using AbilityEvents;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities
{
    /// <summary>
    /// For Buildings, Spells, and Charms. These cards activate automatically when some event happens
    /// </summary>
    public abstract class PassiveAbilitySO : CardAbilitySO
    {
        [Header("Passive Ability Settings")]
        [Tooltip("The events that trigger Execute ability on this SO")]
        public AbilityEventType[] eventsThatTriggerThisAbility;
        
        [Header("Event Scope")]
        [Tooltip("True = Listen globally (events anywhere)\nFalse = Listen locally (events on same middleTile only)")]
        public bool isGlobalListener = true;

        [Tooltip("Executes its ability on place (like a spell), along with passively listening")]
        public bool isExecutableOnPlaced = false;
        
        public abstract override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);

        public virtual void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            Debug.LogWarning($"Called base.UndoExecution on {name} when should be using child override");
        }
        
        /// <summary>
        /// Some abilities pass the event_data.custom_data dictionary that holds the tile it was activated on
        /// This function returns the creature on that tile.
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns>Creature Game object on tile in event_data.custom_data</returns>
        protected GameObject GetCreatureObjFromEventDataTile(AbilityEventData eventData)
        {
            MiddleTile tile = eventData.CustomData.Values.First() as MiddleTile;

            if (tile == null)
            {
                Debug.LogError($"Tile passed to {name} is null");
                return null;
            }
            
            if (tile.logicalCreature != null)
            {
                return tile.logicalCreature;
            }
            
            Debug.LogError($"Tile ({tile}) passed to {name} doesn't have a creature on it for some reason");
            return null;
        }
    }
}