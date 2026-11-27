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
        public int defPerCardDrawn;
        
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            PlayerCardTracker playerCardTracker = thisCard.Ext_GetOwningCardTracker();
                        
            int buffAmount = playerCardTracker.numOfCardsDrawnThisTurn * defPerCardDrawn;
            
            // this ability used to be passive
            // but then I learned that scriptable objects can't be used to track things like cards drawn...
            // ...since changing a variable in here writes to the asset and saves even after play ends
            // so instead I'll save those kinds of stats to the card tracker and leave creatures as active
            
            // Debug.Log($"Fortifying {thisCard} by {buffAmount}");
            
            CreatureStats stats = thisCard.GetComponent<CreatureStats>();
            stats.UpdateCreatureDefense(buffAmount, buff: true);
            
            // AnimateAbilityExecute(thisCard);
        }
        
        [Header("Rejection Message")]
        [SerializeField] [TextArea] private string rejectMessage;
    
        public override bool ValidateAbility(GameObject thisCard)
        {
            PlayerCardTracker playerCardTracker = thisCard.Ext_GetOwningCardTracker();

            int cards = playerCardTracker.numOfCardsDrawnThisTurn;

            if (cards == 0)
            {
                FloatCustomText_FromValidationReject(thisCard, rejectMessage);
                return false;
            }

            return true;
        }
    }
}
