using AbilityEvents;
using UnityEngine;

namespace CardScripts.Abilities.CreatureAbilities.Draw.FortifySelf_ForEachCardDrawnThisTurn_Creature
{
    [CreateAssetMenu(fileName = "FortifySelf_ForEachCardDrawnThisTurn_Creature",
        menuName = "Abilities/Creature/Draw/FortifySelf_ForEachCardDrawnThisTurn_Creature")]
    public class FortifySelf_ForEachCardDrawnThisTurn_Creature : PassiveAbilitySO
    {
        [Header("Ability Stats")]
        public int numOfCardsDrawnThisTurn;
    
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            Debug.Log($"Triggered {name} on {thisCard.name} via {eventData.EventType}");
        }
    }
}
