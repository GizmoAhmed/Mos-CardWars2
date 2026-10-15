using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DamageOnCast_AddDrainIfDamageDiscardsCreature_Spell", 
    menuName = "Abilities/Spell/DamageOnCast_AddDrainIfDamageDiscardsCreature_Spell")]
public class DamageOnCast_AddDrainIfDamageDiscardsCreature_Spell : CastAbilitySO
{
    public int damage;
    public int drainGained;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureStats = eventData.Ext_GetCreatureStats_FromSpellCastEventData();
        
        if (creatureStats == null)
            return; // error message inside above function
            
        // deal damage
        creatureStats.UpdateCreatureDefense(damage, buff: false);
        
        // movement of hit creature
        CreatureMovement creatureMovement = creatureStats.GetComponent<CreatureMovement>();

        if (creatureMovement.cardState == CardMovement.CardState.Discard) // killed it
        {
            PlayerStats player = thisCard.Ext_GetOwningPlayerStats();
            
            player.drain += drainGained;
        }
    }
}