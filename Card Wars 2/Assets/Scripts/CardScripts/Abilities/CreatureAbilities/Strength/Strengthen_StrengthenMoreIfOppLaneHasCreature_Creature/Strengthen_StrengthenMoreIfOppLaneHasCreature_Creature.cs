using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using Tiles;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Strengthen_StrengthenMoreIfOppLaneHasCreature_Creature",
    menuName = "Abilities/Creature/Strengthen_StrengthenMoreIfOppLaneHasCreature_Creature")]
public class Strengthen_StrengthenMoreIfOppLaneHasCreature_Creature : ActiveAbilitySO
{
    [Tooltip("The lesser amount of strength gotten from an EMPTY opposing lane")]
    public int defaultStrength;

    [Tooltip("The greater amount of strength gotten from an opposing lane with a creature")]
    public int bonusStrength;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (bonusStrength <= defaultStrength)
        {
            Debug.LogError(
                $"<color=orange>{this} on {thisCard.gameObject}</color>: Bonus strength <= than default strength, this makes this ability <color=red>pointless</color>");

            return;
        }

        // get creature in opp lane
        MiddleTile thisTile = thisCard.Ext_GetTile() as MiddleTile;
        MiddleTile acrossTile = thisTile.Ext_GetTileAcrossFromThisTile() as MiddleTile;

        GameObject acrossCreature = acrossTile.logicalCreature;

        int buffAmount = 0;

        buffAmount = acrossCreature == null ? defaultStrength : bonusStrength;

        CreatureStats thisCreature = thisCard.GetComponent<CreatureStats>();

        thisCreature.UpdateCreatureStrength(amount: buffAmount, buff: true);

        AnimateAbilityExecute(thisCard);

        if (acrossCreature != null) // jiggle the other one to I guess
        {
            AnimateAbilityExecute(acrossCreature);
        }
    }
}