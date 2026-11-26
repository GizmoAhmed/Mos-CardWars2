using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "IncreaseRandomOppCharmSoul_Creature", 
    menuName = "Abilities/Creature/IncreaseRandomOppCharmSoul_Creature")]
public class IncreaseRandomOppCharmSoul_Creature : ActiveAbilitySO
{
    public int soulIncrease;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        List<CharmMovement> oppCharms = thisCard.Ext_GetAllOppActiveCharms();

        if (oppCharms.Count == 0) return;

        // pick random opp charm
        CharmMovement randomOppCharm = oppCharms.Ext_GetRandomFromList();
        
        // get stats
        CardStats charmStats = randomOppCharm.GetComponent<CardStats>();
        
        // increase soul 
        charmStats.UpdateSyncSoulToPlayer(diff: soulIncrease, source: thisCard.transform);
    }
    
    [Header("Rejection Message")]
    [SerializeField] [TextArea] private string rejectMessage;

    public override bool ValidateAbility(GameObject thisCard)
    {
        List<CharmMovement> oppCharms = thisCard.Ext_GetAllOppActiveCharms();

        if (oppCharms.Count == 0)
        {
            FloatCustomText_FromValidationReject(thisCard, rejectMessage);
            return false;
        }
        
        return true;
    }
}