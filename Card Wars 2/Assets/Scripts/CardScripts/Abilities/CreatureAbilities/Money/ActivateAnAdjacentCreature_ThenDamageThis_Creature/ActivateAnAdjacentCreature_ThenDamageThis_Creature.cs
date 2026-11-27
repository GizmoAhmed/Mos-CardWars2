using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "ActivateAnAdjacentCreature_ThenDamageThis_Creature", 
    menuName = "Abilities/Creature/ActivateAnAdjacentCreature_ThenDamageThis_Creature")]
public class ActivateAnAdjacentCreature_ThenDamageThis_Creature : ActiveAbilitySO
{
    public int selfDamage;

    public AdjacentSide Side;

    public enum AdjacentSide
    {
        Left,
        Right
    }
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        string leftOrRight = Side == AdjacentSide.Left ? "left" : "right";
        
        MiddleTile adj = thisCard.Ext_GetEitherAdjacentTile(leftOrRight);

        if (adj == null || adj.logicalCreature == null) // in a position where there is not right or left, or simply no creature on that tile 
        {
            thisCard.GetComponent<CreatureStats>().UpdateCreatureDefense(amount: selfDamage, buff: false); // damage this anyway
            return;
        } 

        // look at adjacent creature
        GameObject adjCreature = adj.logicalCreature;

        // damage thisCard
        thisCard.GetComponent<CreatureStats>().UpdateCreatureDefense(amount: selfDamage, buff: false);
        
        // activate adj creature
        adjCreature.GetComponent<CreatureStats>().RemoteActivateThisCreature(source: thisCard.transform);

        // AnimateAbilityExecute(thisCard);
    }
}