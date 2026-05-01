using AbilityEvents;
using CardScripts.CardStatss;
using UnityEngine;

namespace CardScripts.Abilities.CreatureAbilities.Script
{
    [CreateAssetMenu(fileName = "BuffStr_Self_Floop_Creature", menuName = "Abilities/Creature/BuffStr_Self_Floop_Creature")]
    public class BuffStr_Self_Floop_Creature : ActiveAbilitySO
    {
        public int baseStrengthBuffAmount;
        
        // public bool Target todo creature abilities that lets players choose which middleTile to effect

        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            // Debug.Log($"Self buffing strength (+{baseStrengthBuffAmount}) on {thisCard.name}");
            CreatureStats creatureStats = thisCard.GetComponent<CreatureStats>();

            if (creatureStats == null)
            {
                Debug.LogError($"Self Strength Buff can't execute on ({thisCard.name}) because creature stats is null");
                return;
            }
            
            creatureStats.ChangeCreatureStrength(baseStrengthBuffAmount, buff:true);
        }
    }
}