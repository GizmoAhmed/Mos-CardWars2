using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using UnityEngine;

namespace CardScripts.Abilities.RuneAbilities.DoubleBuffsNerfs_OnBoundCreature_Rune
{
    [CreateAssetMenu(fileName = "DoubleBuffsNerfs_OnBoundCreature_Rune",
        menuName = "Abilities/Runes/DoubleBuffsNerfs_OnBoundCreature_Rune")]
    public class DoubleBuffsNerfs_OnBoundCreature_Rune : PassiveAbilitySO
    {
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats creatureStats = GetCreatureObjFromEventDataTile(eventData).GetComponent<CreatureStats>();

            // double whatever the current mult is, usually 1
            creatureStats.strengthMult *= 2;
            creatureStats.defenseMult *= 2;
        }

        public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            RuneMovement runeMove = thisCard.GetComponent<RuneMovement>();

            CreatureStats boundCreatureStats = runeMove.creatureBoundTo.GetComponent<CreatureStats>();

            boundCreatureStats.strengthMult /= 2;
            boundCreatureStats.defenseMult /= 2;
        }

        public void OnValidate()
        {
            if (!isExecutableOnPlaced)
            {
                Debug.LogError($"{name} needs to be executable on place");
            }
        }
    }
}