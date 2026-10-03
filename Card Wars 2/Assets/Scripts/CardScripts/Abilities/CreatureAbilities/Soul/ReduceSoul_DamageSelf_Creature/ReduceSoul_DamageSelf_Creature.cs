using AbilityEvents;
using CardScripts.CardStatss;
using UnityEngine;

namespace CardScripts.Abilities.CreatureAbilities.Soul.ReduceSoul_DamageSelf_Creature
{
    [CreateAssetMenu(
        fileName = "ReduceSoul_DamageSelf_Creature", 
        menuName = "Abilities/Creature/ReduceSoul_DamageSelf_Creature")]
    public class ReduceSoul_DamageSelf_Creature : ActiveAbilitySO
    {
        public int damage;
        public int soulReduction;
        
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats stats = thisCard.GetComponent<CreatureStats>();
        
            stats.UpdateCreatureDefense(amount: damage, buff:false);
            stats.UpdateSyncSoulToPlayer(-soulReduction);
        }
    }
}