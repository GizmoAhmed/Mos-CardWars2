using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using PlayerStuff;
using UnityEngine;

namespace CardScripts.Abilities.CharmAbilities.Scripts
{
    [CreateAssetMenu(fileName = "GiveFreeDrawOfferingOnPlace", menuName = "Abilities/Charm/GiveFreeDrawOfferingOnPlace")]
    public class GiveFreeDrawOfferingOnPlace : PassiveAbilitySO
    {
        public int offeringsGiven = 1;
        
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            PlayerStats playerStats = thisCard.GetComponent<CharmMovement>().thisCardOwnerPlayerStats;
            
            playerStats.freeCardsOffered += offeringsGiven;
            
            Debug.Log($"{thisCard.name} ({name}) is giving {playerStats.gameObject.name} + {offeringsGiven} offering.\n{playerStats.gameObject.name} now has {playerStats.freeCardsOffered} offerings");
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