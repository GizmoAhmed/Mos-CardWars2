using AbilityEvents;
using UnityEngine;

namespace CardScripts.Abilities.BuildingAbilities.DoubleBuffOnCreature_AsLongAsWithinSoulRatio_Building
{
    [CreateAssetMenu(fileName = "DoubleBuffOnCreature_AsLongAsWithinSoulRatio_Building", menuName = "Abilities/Building/DoubleBuffOnCreature_AsLongAsWithinSoulRatio_Building")]
    public class DoubleBuffOnCreature_AsLongAsWithinSoulRatio_Building : PassiveAbilitySO
    {
        [Range(0f, 1f)]
        public float soulRatio;
        
        // execute on place, soul change
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            int x = 9;
            // check owning players soul

            // if under, double
            // if over, reset
        }

        public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            // reset mult
            int x = 9;
        }
    }
}
