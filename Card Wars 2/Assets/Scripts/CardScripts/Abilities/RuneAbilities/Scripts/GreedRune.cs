using AbilityEvents;
using CardScripts.CardMovements;
using PlayerStuff;
using UnityEngine;

namespace CardScripts.Abilities.RuneAbilities.Scripts
{
    [CreateAssetMenu(fileName = "Greed_MoneyOnBuff", menuName = "Abilities/Runes/Greed_MoneyOnBuff")]
    public class GreedRune : PassiveAbilitySO
    {
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            if (eventData.Value <= 0)
            {
                Debug.LogWarning($"{thisCard.name} was sent an unusable stat change value to use. Value received ({eventData.Value}).");
                return;
            }
            
            // todo check if card that your buffing actually has this rune on it
            Debug.Log($"{thisCard.name} was triggered via {eventData.EventType}, greed-ing ({eventData.Value}) on {eventData.CardToBeAffected}...");
            
            PlayerStats playerStats = thisCard.GetComponent<CreatureMovement>().thisCardOwnerPlayerStats;
            
            // todo what does losing stats look like?
            playerStats.money += eventData.Value; // give player money equal to stat gain
        }
        
        public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            // does nothing because rune
        }
    }
}
