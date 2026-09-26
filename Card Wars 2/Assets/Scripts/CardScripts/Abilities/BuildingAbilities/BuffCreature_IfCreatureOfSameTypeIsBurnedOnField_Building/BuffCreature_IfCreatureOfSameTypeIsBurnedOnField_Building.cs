using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BuffCreature_IfCreatureOfSameTypeIsBurnedOnField_Building", 
    menuName = "Abilities/Building/BuffCreature_IfCreatureOfSameTypeIsBurnedOnField_Building")]
public class BuffCreature_IfCreatureOfSameTypeIsBurnedOnField_Building : PassiveAbilitySO
{
    // TODO: Add your ability parameters here
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get burned creature
        int x = 9;

        // check if burned creature was yours

        // compare elements between thisCard and burned creature

        // if true, buff, else, return
    }
}