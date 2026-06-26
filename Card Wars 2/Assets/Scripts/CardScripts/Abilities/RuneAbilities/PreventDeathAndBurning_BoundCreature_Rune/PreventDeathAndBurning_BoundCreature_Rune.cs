using System.Linq;
using AbilityEvents;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Extensions;
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
        }

        public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats boundCreatureStats = thisCard.GetCreatureStats_FromBoundRune_Ext();

            if (boundCreatureStats == null)
            {
                // debug message for error check already done in above function, thank u extension methods
                // Debug.LogError($"Creature bound by this rune ({thisCard}) was found null");
                return;
            }
            
            if (!boundCreatureStats.immortal) // base case to stop stack overflow, todo most likely band-aid solution
            {
                // if true, then we can assume undo was already called in a previous discard
                // this stops the potential stack overflow loop
                return;
            }
            
            // goes back to being killable and burnable
            boundCreatureStats.canBeBurned = true;
            boundCreatureStats.immortal = false;

            // check if dead, if sloth is removed and defense is negative, the creature should just die
            if (boundCreatureStats.defense <= 0) // removing sloth of negative defense creature? kill it
            {
                // below was* causing stack overflow, since discard would come back to this execute in a loop, since discarding creature discards this rune
                boundCreatureStats.GetComponent<CreatureMovement>().ServerDiscard();

                // boundCreatureStats.ChangeCreatureDefense(0, false); // this function discards them as well
            }
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