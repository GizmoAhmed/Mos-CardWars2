using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "IncreaseAcrossCreatureBurnCost_Creature", 
    menuName = "Abilities/Creature/IncreaseAcrossCreatureBurnCost_Creature")]
public class IncreaseAcrossCreatureBurnCost_Creature : ActiveAbilitySO
{
    public int costIncrease;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        MiddleTile acrossTile = thisCard.Ext_GetTileAcrossFromThisCard();

        if (acrossTile.logicalCreature == null) return; // no creature, return

        CreatureStats acrossStats = acrossTile.logicalCreature.GetComponent<CreatureStats>();
        
        // AnimateAbilityExecute(thisCard);
        
        acrossStats.UpdateBurnCost(amount: costIncrease, increase: true, source: thisCard.transform);
    }

    [Header("Rejection Message")]
    [SerializeField] [TextArea] private string rejectMessage;

    public override bool ValidateAbility(GameObject thisCard)
    {
        MiddleTile acrossTile = thisCard.Ext_GetTileAcrossFromThisCard();

        if (acrossTile.logicalCreature == null)
        {
            FloatCustomText_FromValidationReject(thisCard, rejectMessage);
            return false;
        }

        return true;
    }
}