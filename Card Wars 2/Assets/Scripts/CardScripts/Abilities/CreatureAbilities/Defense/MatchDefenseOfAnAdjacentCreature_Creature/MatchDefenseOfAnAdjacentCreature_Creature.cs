using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using Tiles;
using UnityEngine;

[CreateAssetMenu(
    fileName = "MatchDefenseOfAnAdjacentCreature_Creature",
    menuName = "Abilities/Creature/MatchDefenseOfAnAdjacentCreature_Creature")]
public class MatchDefenseOfAnAdjacentCreature_Creature : ActiveAbilitySO
{
    public AdjacentSide Side;

    public enum AdjacentSide
    {
        Left,
        Right
    }

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        string leftOrRight = Side == AdjacentSide.Left ? "left" : "right";

        MiddleTile adj = thisCard.GetEitherAdjacentTile(leftOrRight);

        if (adj == null) return; // in a position where there is not right or left 

        // look at adjacent creature
        GameObject adjCreature = adj.logicalCreature;

        if (adjCreature == null)
        {
            return; // no creature on this tile
        }

        CreatureStats thisCreature = thisCard.GetComponent<CreatureStats>();
        CreatureStats adjStats = adjCreature.GetComponent<CreatureStats>();

        if (adjStats.defense > thisCreature.defense) // match up
        {
            thisCreature.UpdateCreatureDefense(
                amount: adjStats.defense - thisCreature.defense, 
                buff: true,
                applyMult: false);
        }
        else // match down
        {
            thisCreature.UpdateCreatureDefense(
                amount: thisCreature.defense - adjStats.defense, 
                buff: false,
                applyMult: false);
        }
        
        AnimateAbilityExecute(thisCard);
        AnimateAbilityExecute(adjCreature);
    }
}