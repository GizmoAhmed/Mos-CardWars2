using System.Linq;
using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Tiles;
using UnityEngine;

namespace CardScripts.Abilities.RuneAbilities.PreventDeathAndBurning_BoundCreature_Rune
{
    [CreateAssetMenu(fileName = "PreventDeathAndBurning_BoundCreature_Rune",
        menuName = "Abilities/Runes/PreventDeathAndBurning_BoundCreature_Rune")]
    public class PreventDeathAndBurning_BoundCreature_Rune : PassiveAbilitySO
    {
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats creatureStats = GetCreatureObjFromEventDataTile(eventData).GetComponent<CreatureStats>();

            // can't be killed and can't be burned
            creatureStats.immortal = true;
            creatureStats.canBeBurned = false;

            // Debug.Log($"<color=blue>Sloth immortalizing {thisCard}</color>");
        }

        public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            RuneMovement runeMove = thisCard.GetComponent<RuneMovement>();

            CreatureStats boundCreature = runeMove.creatureBoundTo.GetComponent<CreatureStats>();
            
            if (boundCreature == null)
            {
                Debug.LogError($"Creature bound by this rune ({thisCard}) was found null");
                return;
            }

            // goes back to being killable and burnable
            boundCreature.canBeBurned = true;
            boundCreature.immortal = false;
            
            // todo check if dead, if sloth is removed and defense is negative, the creature should just die

           // Debug.Log($"<color=blue>Sloth DE-immortalizing {thisCard}</color>");
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