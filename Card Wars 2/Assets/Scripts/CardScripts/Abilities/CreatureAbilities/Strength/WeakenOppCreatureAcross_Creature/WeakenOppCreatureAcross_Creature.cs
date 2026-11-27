using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using Tiles;
using UnityEngine;

[CreateAssetMenu(
    fileName = "WeakenOppCreatureAcross_Creature",
    menuName = "Abilities/Creature/Strength/WeakenOppCreatureAcross_Creature")]
public class WeakenOppCreatureAcross_Creature : ActiveAbilitySO
{
    public int weaken;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        MiddleTile acrossTile = thisCard.Ext_GetTileAcrossFromThisCard();

        if (acrossTile.logicalCreature == null) return; // no creature, return

        CreatureStats acrossStats = acrossTile.logicalCreature.GetComponent<CreatureStats>();
        
        // AnimateAbilityExecute(thisCard);
        
        acrossStats.UpdateCreatureStrength(amount: weaken, buff: false, applyMult: true, source: thisCard.transform);
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