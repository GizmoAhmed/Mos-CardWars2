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
        
        // todo have runebase save the creatures it's binded to, then get rune base component from this card to compare
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            if (eventData.Value <= 0)
            {
                Debug.LogWarning($"GluttonyRuneDoubleBuffs was sent an unusable stat change value to use. Value received ({eventData.Value}).");
                return;
            }
            
            // todo check if card that your buffing actually has this rune on it
            Debug.Log($"{thisCard.name} was triggered via {eventData.EventType}, doubling buff received ({eventData.Value}) on {eventData.CardToBeAffected}...");
        }
    }
}