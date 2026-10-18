using System.Linq;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardData;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "DamageOppCreatures_OfSameElementOfCreatureOnThisTile_Building", 
    menuName = "Abilities/Building/DamageOppCreatures_OfSameElementOfCreatureOnThisTile_Building")]
public class DamageOppCreatures_OfSameElementOfCreatureOnThisTile_Building : PassiveAbilitySO
{
    public int damage;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureOnTile = thisCard.Ext_GetCreatureStats_FromSharedBuildingsTile();
        
        if (creatureOnTile == null) return;
        
        CreatureDataSO.Element thisElement = creatureOnTile.element;

        foreach (var oppCreature in thisCard.Ext_GetAllOpponentsActiveCreatures()
                     .Where(oppCreature => oppCreature.ElementMatch(thisElement))) 
                        // use LINQ to only get matching elements
        {
            oppCreature.UpdateCreatureDefense(amount: damage, buff: false);
            Debug.Log($"{oppCreature.gameObject.name} matches element with {creatureOnTile.gameObject.name}." +
                      $" Damaging: {oppCreature.gameObject.name}");
        }
    }
}