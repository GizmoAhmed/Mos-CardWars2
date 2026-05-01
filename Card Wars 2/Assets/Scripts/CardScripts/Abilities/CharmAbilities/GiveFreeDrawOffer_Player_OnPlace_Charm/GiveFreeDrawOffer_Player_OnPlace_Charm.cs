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
            
            playerStats.freeCardsOffered += offeringsGiven;
            
            Debug.Log($"<color=blue>{thisCard.name} ({name}) is giving {playerStats.gameObject.name} + {offeringsGiven} offering.\n{playerStats.gameObject.name} now has {playerStats.freeCardsOffered} offerings</color>");
        }

        public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            PlayerStats playerStats = thisCard.GetComponent<CharmMovement>().thisCardOwnerPlayerStats;

            playerStats.freeCardsOffered -= offeringsGiven;
            
            Debug.Log($"{thisCard.name} ({name}) is removing {playerStats.gameObject.name} - {offeringsGiven} offering.\n{playerStats.gameObject.name} now has {playerStats.freeCardsOffered} offerings");
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