using System;
using System.Collections.Generic;
using System.Linq;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStats_Folder;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "IncreaseRandomSpellUse_IfYouHaveNegStrCreature_Creature", 
    menuName = "Abilities/Creature/IncreaseRandomSpellUse_IfYouHaveNegStrCreature_Creature")]
public class IncreaseRandomSpellUse_IfYouHaveNegStrCreature_Creature : ActiveAbilitySO
{
    [Tooltip("Percentage increase of spell use")]
    [Range(1, 100)] public int increasePercentage;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        List<CreatureStats> creatures = thisCard.Ext_GetAllActiveCreaturesForThisPlayer();

        bool atLeastOneCreatureWithNegativeStr = 
            creatures.Any(creature => creature.strength < 0);

        // no creatures are negative
        if (atLeastOneCreatureWithNegativeStr == false) return;
        
        IncreaseRandomConsumableUses_ByPercentage(card: thisCard, percentage: increasePercentage);
    }
    
    [Header("Rejection Message")]
    [SerializeField] [TextArea] private string rejectMessage;
    
    public override bool ValidateAbility(GameObject thisCard)
    {
        if (!base.ValidateAbility(thisCard)) 
        { 
            return false; // base checks fail
        }
        // if base checks pass, continue ↓
        
        List<CreatureStats> creatures = thisCard.Ext_GetAllActiveCreaturesForThisPlayer();

        bool atLeastOneCreatureWithNegativeStr = 
            creatures.Any(creature => creature.strength < 0);

        // no creatures are negative
        if (atLeastOneCreatureWithNegativeStr == false)
        {
            FloatCustomText_FromValidationReject(thisCard,"No Creatures with neg Strength");
            return false;
        }
        
        return ValidateRandomSpellUsesUpdate(thisCard);
    }
}