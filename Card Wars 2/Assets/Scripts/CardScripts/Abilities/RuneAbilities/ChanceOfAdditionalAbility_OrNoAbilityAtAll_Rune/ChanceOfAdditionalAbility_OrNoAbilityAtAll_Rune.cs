using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[CreateAssetMenu(
    fileName = "ChanceOfAdditionalAbility_OrNoAbilityAtAll_Rune", 
    menuName = "Abilities/Rune/ChanceOfAdditionalAbility_OrNoAbilityAtAll_Rune")]
public class ChanceOfAdditionalAbility_OrNoAbilityAtAll_Rune : PassiveAbilitySO
{
    [Header("Chance to Hit")]
    [Range(1, 100)] public int chanceToDouble;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creature = GetCreatureObjFromEventDataTile(eventData).GetComponent<CreatureStats>();

        creature.isGamblingAbility = true;
        creature.gambleChance = chanceToDouble;
    }

    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creature = thisCard.GetCreatureStats_FromBoundRune_Ext();
        creature.isGamblingAbility = false;
        creature.gambleChance = 0;
    }
}