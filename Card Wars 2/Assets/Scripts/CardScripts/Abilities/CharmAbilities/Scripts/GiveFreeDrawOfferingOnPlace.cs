using AbilityEvents;
using UnityEngine;

namespace CardScripts.Abilities.CharmAbilities.Scripts
{
    [CreateAssetMenu(fileName = "GiveFreeDrawOfferingOnPlace", menuName = "Abilities/Charm/GiveFreeDrawOfferingOnPlace")]
    public class GiveFreeDrawOfferingOnPlace : PassiveAbilitySO
    {
        public int offeringsGiven = 1;
        
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            // give player offering
        }

        public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            // remove offering
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