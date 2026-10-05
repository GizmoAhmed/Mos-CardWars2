using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DestroyABuilding_Spell", 
    menuName = "Abilities/Spell/DestroyABuilding_Spell")]
public class DestroyABuilding_Spell : CastAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        BuildingMovement building = eventData.Ext_GetBuilding_FromSpellCastEventData();

        if (building != null)
        {
            building.ServerDiscard(); // destroy
        }
    }
}