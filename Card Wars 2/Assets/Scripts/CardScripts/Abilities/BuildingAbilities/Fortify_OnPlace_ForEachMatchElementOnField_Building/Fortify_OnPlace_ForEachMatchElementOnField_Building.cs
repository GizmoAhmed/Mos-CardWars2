using System.Collections.Generic;
using System.Linq;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardData;
using CardScripts.CardStatss;
using Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = "Fortify_OnPlace_ForEachMatchElementOnField_Building", menuName = "Abilities/Building/Fortify_OnPlace_ForEachMatchElementOnField_Building")]
public class Fortify_OnPlace_ForEachMatchElementOnField_Building : PassiveAbilitySO
{
    public int DefensePerMatch;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureOnTile = GetCreature_FromTileInEventData(thisCard, eventData);

        if (creatureOnTile == null) return;

        CreatureDataSO.Element element = ((CreatureDataSO)creatureOnTile.cardData).element;

        // Combine both lists and count matching elements in one query
        int matches = thisCard.Ext_GetAllActiveCreaturesForThisPlayer()
            .Concat(thisCard.Ext_GetAllOpponentsActiveCreatures())
            .Count(c => c != creatureOnTile && // don't count the creature being buffed
                        c.cardData is CreatureDataSO data && 
                        data.element == element);

        if (matches == 0)
        {
            Debug.Log($"{name}: No matching elements found on field, can't buff {creatureOnTile.gameObject.name}");
            return;
        }

        int totalBuff = DefensePerMatch * matches;

        creatureOnTile.ChangeCreatureDefense(totalBuff, buff: true);

        Debug.Log($"{name} on {thisCard.name} buffed {creatureOnTile.gameObject.name} " +
                  $"by {totalBuff} defense ({matches} matches x {DefensePerMatch})");
    }
}
