using System.Collections.Generic;
using System.Linq;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardData;
using CardScripts.CardStatss;
using Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = "ReduceSoulCost_BlockBothStatBuffs_OnPlace_Building", menuName = "Abilities/Building/ReduceSoulCost_BlockBothStatBuffs_OnPlace_Building")]
public class ReduceSoulCost_BlockBothStatBuffs_OnPlace_Building : PassiveAbilitySO
{
    public int soulReduction;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureOnTile = GetCreature_FromTileInEventData(thisCard, eventData);

        if (creatureOnTile == null) return;

        creatureOnTile.canBuffDefense += 1;
        creatureOnTile.canBuffStrength += 1;
        
        creatureOnTile.soulUse -= soulReduction;
    }

    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureOnTile = GetCreature_FromTileInEventData(thisCard, eventData);

        if (creatureOnTile == null) return;

        // literally just reverse of what execute did
        creatureOnTile.canBuffDefense -= 1;
        creatureOnTile.canBuffStrength -= 1;
        
        creatureOnTile.soulUse += soulReduction;
        
    }
}
