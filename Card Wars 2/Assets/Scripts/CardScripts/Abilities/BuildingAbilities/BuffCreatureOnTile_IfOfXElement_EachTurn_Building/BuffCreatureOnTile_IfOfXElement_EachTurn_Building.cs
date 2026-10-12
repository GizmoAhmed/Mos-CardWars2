using AbilityEvents;
using CardScripts.CardData;
using CardScripts.CardStatss;
using Extensions;
using UnityEngine;

namespace CardScripts.Abilities.BuildingAbilities.BuffCreatureOnTile_IfOfXElement_EachTurn_Building
{
    [CreateAssetMenu(
        fileName = "BuffCreatureOnTile_IfOfXElement_EachTurn_Building", 
        menuName = "Abilities/Building/BuffCreatureOnTile_IfOfXElement_EachTurn_Building")]
    public class BuffCreatureOnTile_IfOfXElement_EachTurn_Building : PassiveAbilitySO
    {
        [Tooltip("Element to buff")]
        public CreatureDataSO.Element elementToBuff;
    
        public int strength;
        public int defense;
        
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats creatureOnTile = thisCard.Ext_GetCreatureStats_FromSharedBuildingsTile();

            if (creatureOnTile == null) return;

            if (creatureOnTile.ElementMatch(elementToBuff)) // if match, buff
            {
                creatureOnTile.UpdateCreatureDefense(amount: defense, buff: true);
                creatureOnTile.UpdateCreatureStrength(amount: strength, buff: true);

                AnimateAbilityExecute(thisCard);
            }
        }
    }
}