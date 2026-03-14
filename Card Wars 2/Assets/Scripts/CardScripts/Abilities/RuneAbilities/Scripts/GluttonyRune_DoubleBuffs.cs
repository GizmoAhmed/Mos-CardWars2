using AbilityEvents;
using UnityEngine;

namespace CardScripts.Abilities.RuneAbilities.Scripts
{
    [CreateAssetMenu(fileName = "GluttonyRuneDoubleBuffs", menuName = "Abilities/Passive Abilities/Runes/GluttonyRuneDoubleBuffs")]
    public class GluttonyRuneDoubleBuffs : PassiveAbilitySO
    {
        public int buffMultiplier = 2;
        
        // public int DebuffMultiplier;
        // public int CurrentBuffAmountSinceBase;
        
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            if (eventData.value <= 0)
            {
                Debug.LogWarning($"GluttonyRuneDoubleBuffs was sent an unusable stat change value to use. Value received ({eventData.value}).");
                return;
            }
            // todo check if card that your buffing actually has this rune on it
            Debug.Log($"{thisCard.name} was triggered via {eventData.EventType}, doubling buff received ({eventData.value}) on {eventData.cardToBeAffected}...");
        }
    }
}