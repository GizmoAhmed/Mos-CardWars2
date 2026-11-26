using AbilityEvents;
using CardScripts.CardMovements;
using PlayerStuff;
using UnityEngine;

namespace CardScripts.Abilities.CharmAbilities.Scripts
{
    [CreateAssetMenu(fileName = "GiveFreeDrawOffer_Player_OnPlace_Charm", menuName = "Abilities/Charm/GiveFreeDrawOffer_Player_OnPlace_Charm")]
    public class GiveFreeDrawOffer_Player_OnPlace_Charm : PassiveAbilitySO
    {
        public int offeringsGiven = 1;
        
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            PlayerStats playerStats = thisCard.GetComponent<CharmMovement>().thisCardOwnerPlayerStats;
            
            playerStats.UpdateFreeCardsOffered(amount: offeringsGiven, increase: true);
        }

        public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            PlayerStats playerStats = thisCard.GetComponent<CharmMovement>().thisCardOwnerPlayerStats;

            playerStats.UpdateFreeCardsOffered(amount: offeringsGiven, increase: false);
        }

        public void OnValidate()
        {
            if (!isExecutableOnPlaced)
            {
                Debug.LogError($"{name} needs to be executable on place");
            }
        }
    }
}