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
    fileName = "IncreaseRandomSpellUse_IfYourTotalDefIsLow_Creature",
    menuName = "Abilities/Creature/IncreaseRandomSpellUse_IfYourTotalDefIsLow_Creature")]
public class IncreaseRandomSpellUse_IfYourTotalDefIsLow_Creature : ActiveAbilitySO
{
    public int defThreshold;

    [Tooltip("Percentage increase of spell use")] [Range(1, 100)]
    public int increasePercentage;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        List<CreatureStats> creatures = thisCard.Ext_GetAllActiveCreaturesForThisPlayer();

        int totalDefense = creatures.Sum(creature => creature.defense);

        if (totalDefense >= defThreshold) return; // too much def

        IncreaseRandomConsumableUses_ByPercentage(card: thisCard, percentage: increasePercentage);
    }

    [Header("Rejection Message")] [SerializeField] [TextArea]
    private string rejectMessage;

    public override bool ValidateAbility(GameObject thisCard)
    {
        if (!base.ValidateAbility(thisCard))
        {
            return false; // base checks fail
        }
        // if base checks pass, continue ↓

        List<CreatureStats> creatures = thisCard.Ext_GetAllActiveCreaturesForThisPlayer();

        int totalDefense = creatures.Sum(creature => creature.defense);

        if (totalDefense >= defThreshold)
        {
            FloatCustomText_FromValidationReject(thisCard, $"Total Def Greater than {defThreshold}");
            return false;  // too much def
        }

        return ValidateRandomSpellUsesUpdate(thisCard);
    }
}