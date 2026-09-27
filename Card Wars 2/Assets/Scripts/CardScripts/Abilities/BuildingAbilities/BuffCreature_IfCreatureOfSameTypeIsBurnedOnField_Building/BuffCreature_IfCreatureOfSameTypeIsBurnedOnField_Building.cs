using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardData;
using CardScripts.CardStatss;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BuffCreature_IfCreatureOfSameTypeIsBurnedOnField_Building", 
    menuName = "Abilities/Building/BuffCreature_IfCreatureOfSameTypeIsBurnedOnField_Building")]
public class BuffCreature_IfCreatureOfSameTypeIsBurnedOnField_Building : PassiveAbilitySO
{
    public int fortify;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // see if creature on tile to even buff
        CreatureStats creatureOnThisTile = thisCard.Ext_GetCreatureStats_FromSharedBuildingsTile();

        if (creatureOnThisTile == null)
        {
            return;
        }

        // get burned creature
        CreatureStats burnedCreature = eventData.target.GetComponent<CreatureStats>();

        if (burnedCreature == creatureOnThisTile) // burning the creature on its own tile doesn't count
        {
            return;
        }

        // check if burned creature was yours
        PlayerStats thisPlayer = thisCard.Ext_GetOwningPlayerStats();
        
        bool owned = burnedCreature.gameObject.Ext_IsCardOwnedByThisPlayer(thisPlayer);

        if (!owned) return;

        // compare elements between thisCard and burned creature
        CreatureDataSO.Element elementBurnedCreature = burnedCreature.element;
        
        bool match = creatureOnThisTile.ElementMatch(elementBurnedCreature);
        
        // if true, buff, else, return

        if (match)
        {
            creatureOnThisTile.UpdateCreatureDefense(amount: fortify,buff: true);
            AnimateAbilityExecute(thisCard);
        }
    }
}