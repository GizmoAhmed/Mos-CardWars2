using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardData;
using CardScripts.CardStatss;
using Extensions;
using Tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffBoth_OnCreatureOfXElement_Spell",
    menuName = "Abilities/Spell/BuffBoth_OnCreatureOfXElement_Spell")]
public class BuffBoth_OnCreatureOfXElement_Spell : CastAbilitySO
{
    public int strengthen;
    public int fortify;

    [Header("Element Required to Buff Creature")]
    public CreatureDataSO.Element elementRequirment;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureStats = eventData.Ext_GetCreatureStats_FromSpellCastEventData();

        if (creatureStats == null)
            return; // error message inside above function

        // The spell condition function below should have turned the spell away if that element didn't match, (protecting the player i guess)
        // so this check is redundant, we'll have it anyway i guess.

        if (creatureStats.element == elementRequirment)
        {
            // buff
            creatureStats.ChangeCreatureStrength(strengthen, buff: true);
            creatureStats.ChangeCreatureDefense(fortify, buff: true);
        }
    }

    // called from spell movement, boolean for allowing or denyign spell cast
    public override bool SpecificSpellPlacementConditions(Tile tile)
    {
        Debug.Log($"{name} that was just placed on {tile.gameObject.name}, checking spell req");
        MiddleTile middleTile = tile as MiddleTile;

        if (middleTile == null) // shouldn't be possible because of valid place in spell movement
        {
            Debug.LogError($"{tile.gameObject.name} was expected to be MiddleTile, but was not for some reason");
            return false;
        }

        GameObject creature = middleTile.logicalCreature;

        if (creature == null) // shouldn't be possible because of valid place in spell movement
        {
            Debug.LogError(
                $"{tile.gameObject.name} was expected to have a creature on it, but none was found for some reason");
            return false;
        }

        CreatureStats stats = creature.GetComponent<CreatureStats>();

        // get element of creature on tile
        CreatureDataSO.Element element_ofCreatureOnTile = stats.element;

        // compare
        if (element_ofCreatureOnTile == elementRequirment)
        {
            return true; // allow place
        }
        else
        {
            Debug.LogWarning($"Spell Valid Cast Check: Creature ({creature.name}, {element_ofCreatureOnTile}) does not have the correct element match: ({name}, {elementRequirment})");
            return false; // not matching, deny placement 
        }
    }
}