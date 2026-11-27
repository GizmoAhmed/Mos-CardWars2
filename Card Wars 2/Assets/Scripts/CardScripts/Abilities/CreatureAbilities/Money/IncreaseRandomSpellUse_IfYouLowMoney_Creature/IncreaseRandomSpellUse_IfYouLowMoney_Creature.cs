using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "IncreaseRandomSpellUse_IfYouLowMoney_Creature", 
    menuName = "Abilities/Creature/IncreaseRandomSpellUse_IfYouLowMoney_Creature")]
public class IncreaseRandomSpellUse_IfYouLowMoney_Creature : ActiveAbilitySO
{
    [Tooltip("Percentage increase of spell use")]
    [Range(1, 100)] public int increasePercentage;

    public int lessThanThisAmountOfMoney;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats playerStats = thisCard.Ext_GetOwningPlayerStats();

        // need to have less than this amount of money
        if (playerStats.shards > lessThanThisAmountOfMoney) return;
        
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
        
        PlayerStats playerStats = thisCard.Ext_GetOwningPlayerStats();

        // need to have less than this amount of money
        if (playerStats.shards > lessThanThisAmountOfMoney)
        {
            FloatCustomText_FromValidationReject(thisCard,"Don't have low money");

            return false;
        }

        return ValidateRandomSpellUsesUpdate(thisCard);
    }
}