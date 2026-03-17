using System.Linq;
using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities.BuildingAbilities.Script
{
    [CreateAssetMenu(fileName = "BuffOnPlace", menuName = "Abilities/Building/BuffOnPlace")]
    public class BuffOnPlace : PlaceAbilitySO
    {
        public int baseStrengthBuffAmount;
        public int baseDefenseBuffAmount;

        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            Debug.Log("Executing BuffOnPlace");

            CreatureStats creatureStats;
            
            if (eventData.CustomData != null) // passed from the passive side, automatic trigger event call doesn't pass event data
            {
                Tile placedOnTile = eventData.CustomData["tile"] as Tile;   // execution from placement
                
                if (placedOnTile.logicalCreature != null)               // tile has creature
                {
                    creatureStats = placedOnTile.logicalCreature.GetComponent<CreatureStats>();
                }
                else
                {
                    return; // no creature to buff 
                }
            }
            else
            {
                if (thisCard == eventData.CardToBeAffected)
                {
                    return; // Don't buff self
                }
                
                creatureStats = eventData.CardToBeAffected.GetComponent<CreatureStats>();
                
                // CreatureStats 
                if (creatureStats == null)
                {
                    return; // Not a creature, todo more ability event types (ie SpellCasted) would remove this if statement
                }
            }
            
            creatureStats.ChangeCreatureStrength(baseStrengthBuffAmount, buff: true);
            creatureStats.ChangeCreatureDefense(baseDefenseBuffAmount, buff: true);
        }

        public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            Debug.Log("Undoing BuffOnPlace...");
        }
    }
}