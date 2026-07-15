using AbilityEvents;
using CardScripts.CardStatss;
using Extensions;
using UnityEngine;

namespace CardScripts.Abilities.CreatureAbilities.Defense.FortifySelf_IfLowHandCount_Creature
{
    [CreateAssetMenu(fileName = "FortifySelf_IfLowHandCount_Creature", menuName = "Abilities/Creature/Defense/FortifySelf_IfLowHandCount_Creature")]
    public class FortifySelf_IfLowHandCount_Creature : ActiveAbilitySO
    {
        public int MaxCardsInHandAllowed;
        public int FortifyAmount;
    
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            int thisPlayerHandCount = thisCard.Ext_GetPlayerHandCount();

            // only low hand counts get buffs, this player has too many
            if (thisPlayerHandCount > MaxCardsInHandAllowed)
            {
                Debug.LogWarning($"<color=cyan>{thisCard.name}</color>'s player has a hand count of {thisPlayerHandCount} which is more than {MaxCardsInHandAllowed} cards. <color=orange>Aborting</color> fortify");
                return;
            }
        
            Debug.Log($"<color=cyan>{thisCard.name}</color>'s player has a hand count of {thisPlayerHandCount} which is less than {MaxCardsInHandAllowed} cards. <color=green>Activating</color> fortification");
            // fortify
            CreatureStats creatureStats = thisCard.GetComponent<CreatureStats>();
            creatureStats.ChangeCreatureDefense(FortifyAmount, buff:true);
        }
    }
}
