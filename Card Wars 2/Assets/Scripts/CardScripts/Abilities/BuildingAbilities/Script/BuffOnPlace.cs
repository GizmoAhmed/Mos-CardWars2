using System.Linq;
using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities.BuildingAbilities.Script
{
    [CreateAssetMenu(fileName = "BuffOnPlace", menuName = "Abilities/Building/BuffOnPlace")]
    public class BuffOnPlace : PassiveAbilitySO
    {
        public int baseStrengthBuffAmount;
        public int baseDefenseBuffAmount;

        private BuffOnPlace() // constructor
        {
            isGlobalListener = false;
            eventsThatTriggerThisAbility = new AbilityEventType[] { AbilityEventType.CardPlacedOnTile };
        }

        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            if (thisCard == eventData.cardToBeAffected)
            {
                return; // Don't buff self
            }

            CreatureStats creatureStats = eventData.cardToBeAffected.GetComponent<CreatureStats>();
            if (creatureStats == null)
            {
                return; // Not a creature, todo more ability event types (ie SpellCasted) would remove this if statement
            }
            
            creatureStats.ChangeCreatureStrength(baseStrengthBuffAmount, buff: true);
            creatureStats.ChangeCreatureDefense(baseDefenseBuffAmount, buff: true);
        }

        public void OnValidate()
        {
            base.OnValidate();

            if (!eventsThatTriggerThisAbility.Contains(AbilityEventType.CardPlacedOnTile))
            {
                Debug.LogError($"{name} isn't listening to tile card placements");
            }

            if (isGlobalListener == true)
            {
                Debug.LogError($"{name} shouldn't be listening globally");
            }
        }
    }
}