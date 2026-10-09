using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DrawRandomCharm_FromCharmGroup_DrawnWithExtraSoul_Spell", 
    menuName = "Abilities/Spell/DrawRandomCharm_FromCharmGroup_DrawnWithExtraSoul_Spell")]
public class DrawRandomCharm_FromCharmGroup_DrawnWithExtraSoul_Spell : CastAbilitySO
{
    // TODO: Add your ability parameters here
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        Debug.Log($"Executing on {this} on {thisCard.name}");
    }
}