using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using Tiles;
using UnityEngine;

[CreateAssetMenu(
    fileName = "WeakenOppCreatureAcross_Creature",
    menuName = "Abilities/Creature/WeakenOppCreatureAcross_Creature")]
public class WeakenOppCreatureAcross_Creature : ActiveAbilitySO
{
    public int weaken;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        MiddleTile acrossTile = thisCard.Ext_GetTileAcrossFromThisCard();

        GameObject weakenMe = acrossTile.logicalCreature;
        
        if (weakenMe == null)
        {
            return;
        }
        
        weakenMe.GetComponent<CreatureStats>().UpdateCreatureStrength(amount: weaken, buff: false);
        
        AnimateAbilityExecute(thisCard);
        AnimateAbilityExecute(weakenMe);
    }
}