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
            if (thisCard == eventData.CardOfOrigin)
            {
                Debug.LogWarning($"{thisCard.name} is trying to buff itself");
                return;
            }

            Debug.Log($"{eventData.CardOfOrigin} was placed on {thisCard.name}, attempting to buff {eventData.CardOfOrigin}");
            
            CreatureStats creatureStatsToBuff = eventData.CardOfOrigin.GetComponent<CreatureStats>();

            if (creatureStatsToBuff == null)
            {
                Debug.LogWarning($"Couldn't find CreatureStats component on {eventData.CardOfOrigin.name}");
                return;
            }

            CardMovement move = thisCard.GetComponent<BuildingMovement>();
            CardMovement move2 = creatureStatsToBuff.GetComponent<CardMovement>();
            
            // check if both cards are on the same tile
            /*creatureStatsToBuff.ChangeCreatureStrength(baseStrengthBuffAmount, buff:true);
            creatureStatsToBuff.ChangeCreatureDefense(baseDefenseBuffAmount, buff:true);*/
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