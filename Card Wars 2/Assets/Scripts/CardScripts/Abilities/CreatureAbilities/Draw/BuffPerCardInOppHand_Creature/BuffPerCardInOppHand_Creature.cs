using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BuffPerCardInOppHand_Creature",
    menuName = "Abilities/Creature/BuffPerCardInOppHand_Creature")]
public class BuffPerCardInOppHand_Creature : ActiveAbilitySO
{
    public int strengthPerCardInOppHand;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get opps hand and count
        PlayerCardTracker oppCardTracker = thisCard.Ext_GetOwningPlayerStats().Ext_GetOpponentCardTracker();

        int count = oppCardTracker.Server_GetPlayerHandCount();

        if (count > 0)
        {
            thisCard.GetComponent<CreatureStats>()
                        .UpdateCreatureStrength(amount: strengthPerCardInOppHand * count, buff: true);
            
            AnimateAbilityExecute(thisCard);
        }
    }
}