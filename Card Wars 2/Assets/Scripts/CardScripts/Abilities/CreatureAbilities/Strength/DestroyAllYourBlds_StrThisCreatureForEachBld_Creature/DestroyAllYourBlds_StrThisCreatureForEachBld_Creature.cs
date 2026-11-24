using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "DestroyAllYourBlds_StrThisCreatureForEachBld_Creature", 
    menuName = "Abilities/Creature/Strength/DestroyAllYourBlds_StrThisCreatureForEachBld_Creature")]
public class DestroyAllYourBlds_StrThisCreatureForEachBld_Creature : ActiveAbilitySO
{
    public int strPerBuilding;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        List<BuildingMovement> yourBuilds = thisCard.Ext_GetAllActiveBuildingsForThisPlayer();
        
        int numOfBuilds = yourBuilds.Count;

        if (numOfBuilds == 0) return; // no buildings? no point

        // destroy each building
        foreach (BuildingMovement building in yourBuilds)
        {
            building.ServerDiscard();
        }
        
        CreatureStats creatureStats = thisCard.GetComponent<CreatureStats>();
        
        creatureStats.UpdateCreatureStrength(
            amount: strPerBuilding * numOfBuilds, 
            buff: true, 
            applyMult: true, 
            source: null);
    }

    [Header("Rejection Message")]
    [SerializeField] [TextArea] private string rejectMessage;
    
    public override bool ValidateAbility(GameObject thisCard)
    {
        if (!base.ValidateAbility(thisCard)) 
        { 
            return false; // base checks fail
        }
        // if base checks pass, continue ↓
        
        List<BuildingMovement> yourBuilds = thisCard.Ext_GetAllActiveBuildingsForThisPlayer();
        
        int numOfBuilds = yourBuilds.Count;

        if (numOfBuilds == 0)
        {
            FloatCustomText_FromValidationReject(thisCard, rejectMessage);
            return false;
        }

        return true;
    }
}