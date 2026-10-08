using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "GetFreeDrawOffering_ThenDamageThisCreature_Creature", 
    menuName = "Abilities/Creature/GetFreeDrawOffering_ThenDamageThisCreature_Creature")]
public class GetFreeDrawOffering_ThenDamageThisCreature_Creature : ActiveAbilitySO
{
    public int amountOfExtraOffers;
    public int selfDamage;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats player = thisCard.Ext_GetOwningPlayerStats();

        player.freeCardsOffered += amountOfExtraOffers;
        
        CreatureStats stats = thisCard.GetComponent<CreatureStats>();
        
        stats.UpdateCreatureDefense(amount: selfDamage, buff: false);
        
        AnimateAbilityExecute(thisCard);
    }
}