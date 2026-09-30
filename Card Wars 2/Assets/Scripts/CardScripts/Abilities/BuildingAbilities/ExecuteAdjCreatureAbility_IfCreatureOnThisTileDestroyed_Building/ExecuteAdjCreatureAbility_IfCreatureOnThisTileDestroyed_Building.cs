using AbilityEvents;
using CardScripts.CardStatss;
using Extensions;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities.BuildingAbilities.ExecuteAdjCreatureAbility_IfCreatureOnThisTileDestroyed_Building
{
    [CreateAssetMenu(
        fileName = "ExecuteAdjCreatureAbility_IfCreatureOnThisTileDestroyed_Building", 
        menuName = "Abilities/Building/ExecuteAdjCreatureAbility_IfCreatureOnThisTileDestroyed_Building")]
    public class ExecuteAdjCreatureAbility_IfCreatureOnThisTileDestroyed_Building : PassiveAbilitySO
    {
        public AdjacentSide side;
        
        public enum AdjacentSide
        {   
            Left,
            Right   
        }        
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            string leftOrRight = side == AdjacentSide.Left ? "left" : "right";

            MiddleTile adj = thisCard.GetEitherAdjacentTile(leftOrRight);
        
            if (adj == null) return; // in a position where there is not right or left 

            // look at adjacent creature
            GameObject adjCreature = adj.logicalCreature;

            if (adjCreature == null)
            {
                return; // no creature on this tile
            }

            CreatureStats adjCreatureStats = adjCreature.GetComponent<CreatureStats>();
        
            // todo executing here doesn't follow the normal PlayerStats.cs route, keep that in mind
            adjCreatureStats.ActivateCreatureAbility();
        
            AnimateAbilityExecute(thisCard);
        }
    }
}