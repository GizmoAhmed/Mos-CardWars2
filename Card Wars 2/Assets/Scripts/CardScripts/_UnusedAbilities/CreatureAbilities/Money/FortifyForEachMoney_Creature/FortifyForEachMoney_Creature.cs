using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FortifyForEachMoney_Creature", 
    menuName = "Abilities/Creature/FortifyForEachMoney_Creature")]
public class FortifyForEachMoney_Creature : ActiveAbilitySO
{
    [Tooltip("Ie: If you set 5, then every 5 money, fortify by 1, so 20$ = 4 defense.")]
    public int OneForitfyForThisMuchMoney;    
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats player = thisCard.Ext_GetOwningPlayerStats();

        int shards = player.shards;
        
        int fortify = shards / OneForitfyForThisMuchMoney;

        if (fortify <= 0) return; // no point
        
        CreatureStats creature = thisCard.GetComponent<CreatureStats>();
        
        creature.UpdateCreatureDefense(amount: fortify, buff: true);
        AnimateAbilityExecute(thisCard);
    }
}