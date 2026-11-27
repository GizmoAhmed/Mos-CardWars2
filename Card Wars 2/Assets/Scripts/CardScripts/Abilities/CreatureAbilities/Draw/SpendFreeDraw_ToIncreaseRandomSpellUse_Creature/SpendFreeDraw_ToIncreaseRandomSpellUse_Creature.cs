using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "SpendFreeDraw_ToIncreaseRandomSpellUse_Creature", 
    menuName = "Abilities/Creature/Draw/SpendFreeDraw_ToIncreaseRandomSpellUse_Creature")]
public class SpendFreeDraw_ToIncreaseRandomSpellUse_Creature : ActiveAbilitySO
{
    public int freeDrawsExpenditure;
    
    [Tooltip("Percentage increase of spell use")] [Range(1, 100)]
    public int increasePercentage;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats player = thisCard.Ext_GetOwningPlayerStats();

        if (player.freeDrawsLeft <= 0) return; // no free draws to spend
        
        player.freeDrawsLeft -= freeDrawsExpenditure;
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

        PlayerStats player = thisCard.Ext_GetOwningPlayerStats();

        if (player.freeDrawsLeft <= 0) // not enough draws
        {
            FloatCustomText_FromValidationReject(thisCard, $"No free draws left");
            return false;
        }

        return ValidateRandomSpellUsesUpdate(thisCard);
    }
}