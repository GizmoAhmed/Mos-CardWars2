using CardScripts.CardStatss;
using UnityEngine;

namespace CardScripts.Abilities.CreatureAbilities.Script
{
    [CreateAssetMenu(fileName = "SelfBuffStrength", menuName = "Abilities/Self Buff Strength")]
    public class SelfBuffStrength : CardAbilitySO
    {
        public int baseStrengthBuffAmount;

        public override bool Condition()
        {
            return true; // no condition todo unless there is some kind of ability block
        }

        public override void Execute(GameObject thisCard)
        {
            Debug.Log($"Self buffing strength (+{baseStrengthBuffAmount}) on {thisCard.name}");
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