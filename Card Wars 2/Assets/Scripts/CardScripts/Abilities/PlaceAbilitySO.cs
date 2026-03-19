using AbilityEvents;
using CardScripts.CardStatss;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities
{
    // place ability is a passive ability that only listens to card placement and casts like a spell.
    //  listening and casting result in the same execution.
    // When this card is discard, undo its execution on the affected card
    public abstract class PlaceAbilitySO : PassiveAbilitySO
    {
        void PlaceAbility()
        {
            isGlobalListener = false;
            eventsThatTriggerThisAbility = new AbilityEventType[] { AbilityEventType.CardPlacedOnTile };
        }

        public abstract override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);
        
        public abstract void UndoExecution(GameObject thisCard, AbilityEventData eventData);

        public CreatureStats GetCreature_FromTileInEventData(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats creatureStats;
            
            if (eventData.CustomData != null) // Execution was manual, called via placement in RegisterPassiveAbility, this building was placed on a tile
            {
                Tile placedOnTile = eventData.CustomData["tile"] as Tile;   
                
                if (placedOnTile.logicalCreature != null)               // tile has creature
                {
                    creatureStats = placedOnTile.logicalCreature.GetComponent<CreatureStats>();
                }
                else
                {
                    return null; // no creature to buff 
                }
            }
            else // Execution was passive, triggered by call back from event system, this building was told a creature was placed on it
            {
                if (thisCard == eventData.CardToBeAffected)
                {
                    return null; // Don't buff self
                }
                
                creatureStats = eventData.CardToBeAffected.GetComponent<CreatureStats>();
                
                // CreatureStats 
                if (creatureStats == null)
                {
                    return null; // Not a creature, todo more ability event types (ie SpellCasted) would remove this if statement
                }
            }
            
            return creatureStats;
        }
    }
}