using AbilityEvents;
using CardScripts.Abilities;
using Extensions;
using UnityEngine;
using Tiles;
using CardScripts.CardStatss;

[CreateAssetMenu(
    fileName = "BuffOrNerf_EachTurn_BasedOnAcrossCreatureScore_Building", 
    menuName = "Abilities/Building/BuffOrNerf_EachTurn_BasedOnAcrossCreatureScore_Building")]
public class BuffOrNerf_EachTurn_BasedOnAcrossCreatureScore_Building : PassiveAbilitySO
{
    public int strengthen;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        MiddleTile acrossTile = thisCard.Ext_GetTileAcrossFromThisCard();

        GameObject acrossCreature = acrossTile.logicalCreature;

        if (acrossCreature == null) return; // do nothing

        CreatureStats creatureOnThisTile = thisCard.Ext_GetCreatureStats_FromSharedBuildingsTile();
        
        int thisScore = creatureOnThisTile.score;
        int acrossScore = acrossCreature.GetComponent<CreatureStats>().score;
        
        if (acrossScore > thisScore) // buff
        {
            creatureOnThisTile.UpdateCreatureStrength(amount: strengthen, buff: true);
            AnimateAbilityExecute(thisCard);
        }
        else if (acrossScore < thisScore) // across is weaker, nerf
        {
            creatureOnThisTile.UpdateCreatureStrength(amount: strengthen, buff: false);
            AnimateAbilityExecute(thisCard);
        }
        else // (acrossScore == thisScore)
        {
            // do nothing
        }
    }
}