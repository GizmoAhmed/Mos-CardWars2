using System;
using AbilityEvents;
using CardScripts.CardStatss;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities
{
    [Serializable]
    public abstract class CardAbilitySO : ScriptableObject
    {
        public abstract void ExecuteAbility(GameObject thisCard, AbilityEventData eventData);
        
        /// <summary>
        /// An ability execution calls this when they want the tile the card is sitting ion
        /// </summary>
        /// <param name="thisCard"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public CreatureStats GetCreature_FromTileInEventData(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats creatureStats;
            
            if (eventData.CustomData != null) // Execution was manual, called via placement in RegisterPassiveAbility, this building was placed on a middleTile
            {
                MiddleTile placedOnMiddleTile = eventData.CustomData["middleTile"] as MiddleTile;   
                
                if (placedOnMiddleTile.logicalCreature != null)               // middleTile has creature
                {
                    creatureStats = placedOnMiddleTile.logicalCreature.GetComponent<CreatureStats>();
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