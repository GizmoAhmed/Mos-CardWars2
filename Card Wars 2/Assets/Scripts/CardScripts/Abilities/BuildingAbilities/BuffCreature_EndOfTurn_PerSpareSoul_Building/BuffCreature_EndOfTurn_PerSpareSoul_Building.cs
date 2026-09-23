using AbilityEvents;
using CardScripts.CardStatss;
using Extensions;
using PlayerStuff;
using UnityEngine;

namespace CardScripts.Abilities.BuildingAbilities.BuffCreature_EndOfTurn_PerSpareSoul_Building
{
    [CreateAssetMenu(fileName = "BuffCreature_EndOfTurn_PerSpareSoul_Building", menuName = "Abilities/Building/BuffCreature_EndOfTurn_PerSpareSoul_Building")]
    public class BuffCreature_EndOfTurn_PerSpareSoul_Building : PassiveAbilitySO
    {
        public int fortifyPerSpareSoul;
    
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            // check if creature on tile
            CreatureStats creatureStats = thisCard.Ext_GetCreatureStats_FromSharedBuildingsTile();

            // count spare soul
            PlayerStats player = thisCard.Ext_GetOwningPlayerStats();
        
            int spareSoul = player.maxSoul - player.currentSoul;

            // buff per spare
            creatureStats.UpdateCreatureDefense(fortifyPerSpareSoul * spareSoul, buff: true);
            AnimateAbilityExecute(thisCard);
        }
    }
}
