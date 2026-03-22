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

        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats creatureStats = GetCreature_FromTileInEventData(thisCard, eventData);

            if (creatureStats != null)
            {
                creatureStats.ChangeCreatureStrength(baseStrengthBuffAmount, buff: true);
                creatureStats.ChangeCreatureDefense(baseDefenseBuffAmount, buff: true);
            }
        }

        public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            // Debug.Log("Does nothing....");
        }

        public void OnValidate()
        {
            if (isGlobalListener && !isExecutableOnPlaced)
            {
                Debug.LogError($"{name} needs to be executable on place and a middleTile listener");
            }
        }
    }
}