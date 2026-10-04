using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardData;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BuffYourCreaturesOfXElement_EachTurn_Charm", 
    menuName = "Abilities/Charm/BuffYourCreaturesOfXElement_EachTurn_Charm")]
public class BuffYourCreaturesOfXElement_EachTurn_Charm : PassiveAbilitySO
{
    [Tooltip("Element to buff")]
    public CreatureDataSO.Element elementToBuff;
    
    public int strength;
    public int defense;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get this player's creatures
        List<CreatureStats> creatures = thisCard.Ext_GetAllActiveCreaturesForThisPlayer();

        // buff if they match the element
        foreach (CreatureStats creature in creatures)
        {
            if (creature.ElementMatch(elementToBuff))
            {
                creature.UpdateCreatureDefense(amount: defense, buff: true);
                creature.UpdateCreatureStrength(amount: strength, buff: true);
            }
        }
        
        AnimateAbilityExecute(thisCard);
    }
}