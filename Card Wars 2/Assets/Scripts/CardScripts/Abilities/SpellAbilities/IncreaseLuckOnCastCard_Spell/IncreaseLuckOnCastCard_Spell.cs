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
using UnityEngine.TextCore.Text;

[CreateAssetMenu(
    fileName = "IncreaseLuckOnCastCard_Spell", 
    menuName = "Abilities/Spell/IncreaseLuckOnCastCard_Spell")]
public class IncreaseLuckOnCastCard_Spell : CastAbilitySO
{
    public int luckIncrease;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CardStats cardToBuff;
        
        // pick either a creature or building
        if (eventData.target.TryGetComponent(out MiddleTile mid)) 
        {
            if (mid.logicalCreature == null && mid.logicalBuilding == null)
            {
                Debug.LogError($"{thisCard.name}: No card on this tile. This should not be possible. Check spell specs");
                return;
            }
            
            // no building, but there's a creature
            if (mid.logicalBuilding == null && mid.logicalCreature != null)
            {
                cardToBuff = mid.logicalCreature.GetComponent<CardStats>();
            }
            // building and no creature
            else if (mid.logicalBuilding != null && mid.logicalCreature == null)
            {
                cardToBuff = mid.logicalBuilding.GetComponent<CardStats>();
            }
            else // both there, pick randomly between the two
            {
                bool isHeads = Random.value > 0.5f;

                if (isHeads)
                {
                    cardToBuff =  mid.logicalCreature.GetComponent<CardStats>();
                }
                else
                {
                    cardToBuff = mid.logicalBuilding.GetComponent<CardStats>();
                }
            }
        } 
        
        // pick a random charm if any
        else if (eventData.target.TryGetComponent(out CharmTile charmTile))
        {
            List<CharmMovement> charms = eventData.Ext_GetCharms_FromSpellCastEventData();

            if (charms.Count == 0) // should never trigger but just in case
            {
                Debug.LogError($"{thisCard.name}: No charms found. This should not happen. Check spell specs");
                 return;
            }

            CharmMovement randomCharm = charms.Ext_GetRandomFromList();
            
            cardToBuff = randomCharm.GetComponent<CardStats>();
        }
        else
        {
            return; // just so the below line stops whining
        }

        cardToBuff.UpdateBonusLuck(amount: luckIncrease, buff: true);
    }
}