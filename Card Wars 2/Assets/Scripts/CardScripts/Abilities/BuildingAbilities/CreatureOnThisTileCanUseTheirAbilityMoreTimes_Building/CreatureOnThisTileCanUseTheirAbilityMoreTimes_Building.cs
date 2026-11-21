using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "CreatureOnThisTileCanUseTheirAbilityMoreTimes_Building", 
    menuName = "Abilities/Building/CreatureOnThisTileCanUseTheirAbilityMoreTimes_Building")]
public class CreatureOnThisTileCanUseTheirAbilityMoreTimes_Building : PassiveAbilitySO
{
    public int additionalFloops;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureStats = GetCreature_FromTileInEventData(thisCard, eventData);

        if (creatureStats != null)
        {
            creatureStats.maxFloops += additionalFloops;
            // creatureStats.floopsLeft += additionalFloops;
            
            creatureStats.UpdateFloopsLeft(amount:additionalFloops, increase: true, source: thisCard.transform);
        }
    }
    
    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureStats = GetCreature_FromTileInEventData(thisCard, eventData);

        if (creatureStats != null)
        {
            creatureStats.UpdateFloopsLeft(amount:additionalFloops, increase: false, source: thisCard.transform);

        }
    }
}