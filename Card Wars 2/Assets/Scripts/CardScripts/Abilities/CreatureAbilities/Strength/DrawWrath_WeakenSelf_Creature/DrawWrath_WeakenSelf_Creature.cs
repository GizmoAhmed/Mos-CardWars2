using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardData;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Extensions;
using GameManagement;
using PlayerStuff;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DrawWrath_WeakenSelf_Creature", 
    menuName = "Abilities/Creature/DrawWrath_WeakenSelf_Creature")]
public class DrawWrath_WeakenSelf_Creature : ActiveAbilitySO
{
    public RuneDataSO wrath;
    public int weakenAmount;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (wrath == null)
        {
            Debug.LogError($"{name} on {thisCard.name} is missing <color=red>wrath</color>, and therefore can't spawn it. Aborting...");
            return;
        }

        // draw rune
        PlayerStats player = thisCard.Ext_GetOwningPlayerStats();

        GameObject drawnWrath = DrawCard_GivenCardDataSO(data: wrath, player);
        
        // weaken creature
        CreatureStats creature = thisCard.GetComponent<CreatureStats>();
        
        creature.UpdateCreatureStrength(amount: weakenAmount, buff: false);
        
        // AnimateAbilityExecute(drawnWrath);
        AnimateAbilityExecute(thisCard);
    }
}