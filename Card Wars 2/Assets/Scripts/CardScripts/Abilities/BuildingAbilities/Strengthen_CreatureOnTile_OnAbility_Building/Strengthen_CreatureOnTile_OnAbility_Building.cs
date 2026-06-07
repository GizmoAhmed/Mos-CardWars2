using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using UnityEngine;

[CreateAssetMenu(fileName = "Strengthen_CreatureOnTile_OnAbility_Building", menuName = "Abilities/Building/Strengthen_CreatureOnTile_OnAbility_Building")]
public class Strengthen_CreatureOnTile_OnAbility_Building : PassiveAbilitySO
{
    public int Strength;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        GameObject creature = eventData.target.gameObject;
        
        CreatureStats creatureStats = creature.GetComponent<CreatureStats>();
        
        creatureStats.ChangeCreatureStrength(Strength, buff: true);
    }

}
