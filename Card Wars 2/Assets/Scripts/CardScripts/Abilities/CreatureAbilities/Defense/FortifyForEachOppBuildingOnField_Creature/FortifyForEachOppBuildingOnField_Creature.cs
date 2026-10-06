using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FortifyForEachOppBuildingOnField_Creature", 
    menuName = "Abilities/Creature/FortifyForEachOppBuildingOnField_Creature")]
public class FortifyForEachOppBuildingOnField_Creature : ActiveAbilitySO
{
    public int fortifyPerBuilding;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerCardTracker opp = thisCard.Ext_GetOpponentCardTracker();

        var oppFieldCards = opp.Server_GetThisPlayerAllActiveFieldCards();

        int count = 0;
        
        foreach (var card in oppFieldCards)
        {
            if (card.GetComponent<BuildingMovement>())
            {
                count++;
            }
        }

        if (count == 0) return;

        int fortify = count *  fortifyPerBuilding;
        
        thisCard.GetComponent<CreatureStats>().UpdateCreatureDefense(amount: fortify, buff: true);
        
        AnimateAbilityExecute(thisCard);
    }
}