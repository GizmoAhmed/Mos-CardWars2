using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardData;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BuffYourCreatures_ThatMatchElementWithDrawnCreature_Charm", 
    menuName = "Abilities/Charm/BuffYourCreatures_ThatMatchElementWithDrawnCreature_Charm")]
public class BuffYourCreatures_ThatMatchElementWithDrawnCreature_Charm : PassiveAbilitySO
{
    public int fortify;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get all creatures on field
        List<CreatureStats> yourActiveCreatures = thisCard.Ext_GetAllActiveCreaturesForThisPlayer();

        if (yourActiveCreatures.Count == 0) return; // no point if not active cards to buff
        
        // get element of drawn creature
        CreatureStats drawnCreature = eventData.target.GetComponent<CreatureStats>();

        if (drawnCreature == null) return; // not a creature, don't care
        
        bool yours = drawnCreature.gameObject.Ext_IsSameOwner(thisCard);
        
        if(!yours) return; // not yours, don't care
        
        CreatureDataSO.Element drawnCreatureElement = drawnCreature.element;

        // foreach creature on field, check match, buff if match
        foreach (CreatureStats creature in yourActiveCreatures)
        {
            if (creature.ElementMatch(drawnCreatureElement))
            {
                creature.UpdateCreatureDefense(amount: fortify, buff: true, applyMult: true);
                AnimateAbilityExecute(thisCard);
                AnimateAbilityExecute(creature.gameObject);
            }
        }
    }
}