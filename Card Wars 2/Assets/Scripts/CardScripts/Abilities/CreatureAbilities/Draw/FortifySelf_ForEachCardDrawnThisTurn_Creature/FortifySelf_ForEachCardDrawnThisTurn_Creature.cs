using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Extensions;
using PlayerStuff;
using UnityEngine;

namespace CardScripts.Abilities.CreatureAbilities.Draw.FortifySelf_ForEachCardDrawnThisTurn_Creature
{
    [CreateAssetMenu(fileName = "FortifySelf_ForEachCardDrawnThisTurn_Creature",
        menuName = "Abilities/Creature/Draw/FortifySelf_ForEachCardDrawnThisTurn_Creature")]
    public class FortifySelf_ForEachCardDrawnThisTurn_Creature : ActiveAbilitySO
    {
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            PlayerCardTracker playerCardTracker = thisCard.GetOwningCardTracker_Ext();
                        
            int buffAmount = playerCardTracker.numOfCardsDrawnThisTurn;
            
            // this ability used to be passive
            // but then I learned that scriptable objects can't be used to track things like cards drawn...
            // ...since changing a variable in here writes to the asset and saves even after play ends
            // so instead I'll save those kinds of stats to the card tracker and leave creatures as active
            
            // Debug.Log($"Fortifying {thisCard} by {buffAmount}");
            
            CreatureStats stats = thisCard.GetComponent<CreatureStats>();
            stats.ChangeCreatureDefense(buffAmount, buff: true);
        }
    }
}
