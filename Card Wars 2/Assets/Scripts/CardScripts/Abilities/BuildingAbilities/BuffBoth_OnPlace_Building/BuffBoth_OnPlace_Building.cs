using System.Linq;
using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities.BuildingAbilities.Script
{
    [CreateAssetMenu(fileName = "BuffBoth_OnPlace_Building", menuName = "Abilities/Building/BuffBoth_OnPlace_Building")]
    public class BuffBoth_OnPlace_Building : PassiveAbilitySO
    {
        public int baseStrengthBuffAmount;
        public int baseDefenseBuffAmount;

        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats creatureStats = GetCreature_FromTileInEventData(thisCard, eventData);

            if (creatureStats != null)
            {
                creatureStats.UpdateCreatureStrength(baseStrengthBuffAmount, buff: true);
                creatureStats.ChangeCreatureDefense(baseDefenseBuffAmount, buff: true);
                AnimateAbilityExecute(thisCard);
            }
        }

        public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            // Debug.Log("Does nothing....");
        }
    }
}