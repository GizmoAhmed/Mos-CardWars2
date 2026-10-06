using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEditor.UIElements;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FortifySelf_Creature", 
    menuName = "Abilities/Creature/FortifySelf_Creature")]
public class FortifySelf_Creature : ActiveAbilitySO
{
    public int fortify;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creature = thisCard.GetComponent<CreatureStats>();
        
        creature.UpdateCreatureDefense(amount: fortify, buff: true);
        AnimateAbilityExecute(thisCard);
    }
}