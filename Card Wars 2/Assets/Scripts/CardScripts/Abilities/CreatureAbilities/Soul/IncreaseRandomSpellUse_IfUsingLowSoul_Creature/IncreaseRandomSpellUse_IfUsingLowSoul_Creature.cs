using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "IncreaseRandomSpellUse_IfUsingLowSoul_Creature", 
    menuName = "Abilities/Creature/IncreaseRandomSpellUse_IfUsingLowSoul_Creature")]
public class IncreaseRandomSpellUse_IfUsingLowSoul_Creature : ActiveAbilitySO
{
    [Range(1, 100)] public int soulUseThreshold;

    [Tooltip("Percentage increase of spell use")] [Range(1, 100)]
    public int increasePercentage;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats player = thisCard.Ext_GetOwningPlayerStats();
        
        int currentSoulUse = player.maxSoul - player.currentSoul;
        
        // some cross-multiplication hoodoo claude cooked up
        if (currentSoulUse * 100 > soulUseThreshold * player.maxSoul) return;
        
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
        
        int currentSoulUse = player.maxSoul - player.currentSoul;
        
        // some cross-multiplication hoodoo claude cooked up
        if (currentSoulUse * 100 > soulUseThreshold * player.maxSoul)
        {
            FloatCustomText_FromValidationReject(thisCard, "Using too much soul");
            return false;
        }

        return ValidateRandomSpellUsesUpdate(thisCard);
    }
}