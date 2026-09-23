using AbilityEvents;
using CardScripts.CardStatss;
using Extensions;
using UnityEngine;

namespace CardScripts.Abilities.BuildingAbilities.FortifyPerEmptyLane_CreatureOnTile_OnAbility_Building
{
    [CreateAssetMenu(fileName = "FortifyPerEmptyLane_CreatureOnTile_OnAbility_Building", menuName = "Abilities/Building/FortifyPerEmptyLane_CreatureOnTile_OnAbility_Building")]
    public class FortifyPerEmptyLane_CreatureOnTile_OnAbility_Building : PassiveAbilitySO
    {
        public int defensePerEmptyLane;
    
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            GameObject creature = eventData.target.gameObject;
        
            CreatureStats creatureStats = creature.GetComponent<CreatureStats>();

            int totalTiles = 4;
            int activeCreatureCount = thisCard.Ext_GetAllActiveCreaturesForThisPlayer().Count;

            int rate = totalTiles -  activeCreatureCount;
            
            creatureStats.ChangeCreatureDefense(defensePerEmptyLane * rate, buff:true);
            AnimateAbilityExecute(thisCard);
        }
    }
}
