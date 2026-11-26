using AbilityEvents;
using Extensions;
using PlayerStuff;
using UnityEngine;

namespace CardScripts.Abilities.CreatureAbilities.Draw.TempLowerOppOffering_Creature
{
    [CreateAssetMenu(
        fileName = "TempLowerOppOffering_Creature", 
        menuName = "Abilities/Creature/Draw/TempLowerOppOffering_Creature")]
    public class TempLowerOppOffering_Creature : ActiveAbilitySO
    {
        public int lowerOffering;
        
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            PlayerStats opp = thisCard.Ext_GetOpponentPlayerStats();
            opp.UpdateFreeCardsOffered(amount: lowerOffering, increase: false);

            PlayerCardTracker oppTracker = thisCard.Ext_GetOpponentCardTracker();
            oppTracker.tempFreeOffering_ResetOnPlayerReady += lowerOffering; // negative so that when it adds back on ready, it's back to its og state
        
            AnimateAbilityExecute(thisCard);
        }
    }
}