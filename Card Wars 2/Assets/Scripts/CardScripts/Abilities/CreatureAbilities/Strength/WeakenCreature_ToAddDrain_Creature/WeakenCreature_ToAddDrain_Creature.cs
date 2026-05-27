using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using PlayerStuff;
using UnityEngine;

namespace CardScripts.Abilities.CreatureAbilities.Strength.WeakenCreature_ToAddDrain_Creature
{
    [CreateAssetMenu(fileName = "WeakenCreature_ToAddDrain_Creature", menuName = "Abilities/Creature/Strength/WeakenCreature_ToAddDrain_Creature")]
    public class WeakenCreature_ToAddDrain_Creature : ActiveAbilitySO
    {
        public int drainGainPerWeaken;
    
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats creatureStats = thisCard.GetComponent<CreatureStats>();

            if (creatureStats == null)
            {
                Debug.LogError($"Self Strength Buff can't execute on ({thisCard.name}) because creature stats is null");
                return;
            }

            if (creatureStats.strength > 0)
            {
                PlayerStats player = thisCard.GetComponent<CreatureMovement>().thisCardOwnerPlayerStats;
                player.drain += (creatureStats.strength * drainGainPerWeaken); // gain +X drain per strength weakened
            
                // lose all strength
                creatureStats.ChangeCreatureStrength(creatureStats.strength, false);
            }
        }
    }
}
