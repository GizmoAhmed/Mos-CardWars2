using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities.BuildingAbilities.Script
{
    [CreateAssetMenu(fileName = "BuffOnPlace", menuName = "Abilities/Building/BuffOnPlace")]
    public class BuffOnPlace : CardAbilitySO
    {
        public int baseStrengthBuffAmount;
        public int baseDefenseBuffAmount;

        public override bool Condition()
        {
            return true; // no condition
        }

        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            if (thisCard == eventData.sourceCard)
            {
                return; // Don't buff self
            }

            CreatureStats creatureStats = eventData.sourceCard.GetComponent<CreatureStats>();
            if (creatureStats == null)
            {
                return; // Not a creature
            }

            // Get LOGICAL positions (same on server and all clients!)
            CardMovement buildingMovement = thisCard.GetComponent<CardMovement>();
            CardMovement creatureMovement = eventData.sourceCard.GetComponent<CardMovement>();
    
            // Compare logical positions
            bool sameTile = 
                buildingMovement.logicalRow == creatureMovement.logicalRow &&
                buildingMovement.logicalColumn == creatureMovement.logicalColumn &&
                buildingMovement.logicalPlayerSide == creatureMovement.logicalPlayerSide;
    
            if (sameTile)
            {
                Debug.Log($"Building buffs creature! Both at Row={buildingMovement.logicalRow}, " +
                          $"Col={buildingMovement.logicalColumn}");
        
                creatureStats.ChangeCreatureStrength(baseStrengthBuffAmount, buff: true);
                creatureStats.ChangeCreatureDefense(baseDefenseBuffAmount, buff: true);
            }
        }

        public override void OnValidate()
        {
            base.OnValidate();
            
            if (isPassive == false || triggeringEvents == null)
            {
                Debug.LogError($"{name} should be passive and/or have triggering events");
            }
        }
    }
}