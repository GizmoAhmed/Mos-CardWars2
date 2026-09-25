using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using Extensions;
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
            PlayerStats player = thisCard.Ext_GetOwningPlayerStats();
            
            player.shards += shardsEarned;
        }

        public void OnValidate()
        {
            if (castRequirementType != CastRequirementType.AnyTile)
            {
                Debug.LogError($"{name} should have cast type {CastRequirementType.AnyTile}");
            }
        }
    }
}