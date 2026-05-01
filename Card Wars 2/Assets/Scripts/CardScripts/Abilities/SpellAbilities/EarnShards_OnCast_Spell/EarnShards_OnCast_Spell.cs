using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using PlayerStuff;
using UnityEngine;

namespace CardScripts.Abilities.SpellAbilities.EarnShards_OnCast_Spell
{
    [CreateAssetMenu(fileName = "EarnShards_OnCast_Spell", menuName = "Abilities/Spell/EarnShards_OnCast_Spell")]
    public class EarnShards_OnCast_Spell : CastAbilitySO
    {
        public int shardsEarned;

        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            Debug.Log($"EarnShards_OnCast_Spell, player earned {shardsEarned} shards");
            PlayerStats player = thisCard.GetComponent<SpellMovement>().thisCardOwnerPlayerStats;
            
            player.shards += shardsEarned;
        }

        public void OnValidate()
        {
            if (castRequirementType != CastRequirementType.Anywhere)
            {
                Debug.LogError($"{name} should have cast type {CastRequirementType.Anywhere}");
            }
        }
    }
}