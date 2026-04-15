using AbilityEvents;
using UnityEngine;

namespace CardScripts.Abilities.RuneAbilities.Scripts
{
    [CreateAssetMenu(fileName = "Greed_MoneyOnBuff", menuName = "Abilities/Runes/Greed_MoneyOnBuff")]
    public class GreedRune : PassiveAbilitySO
    {
        // todo have runebase save the creatures it's bound to, then get rune base component from this card to compare
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            if (eventData.Value <= 0)
            {
                Debug.LogWarning($"{thisCard.name} was sent an unusable stat change value to use. Value received ({eventData.Value}).");
                return;
            }
            
            // todo check if card that your buffing actually has this rune on it
            Debug.Log($"{thisCard.name} was triggered via {eventData.EventType}, greed-ing ({eventData.Value}) on {eventData.CardToBeAffected}...");
        }
        
        public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            // does nothing because rune
        }
    }
}
