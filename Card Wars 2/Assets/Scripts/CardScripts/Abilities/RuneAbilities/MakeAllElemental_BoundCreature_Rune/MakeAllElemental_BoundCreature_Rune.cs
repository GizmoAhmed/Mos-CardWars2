using AbilityEvents;
using CardScripts.CardData;
using CardScripts.CardStatss;
using Extensions;
using UnityEngine;

namespace CardScripts.Abilities.RuneAbilities.MakeAllElemental_BoundCreature_Rune
{
    [CreateAssetMenu(fileName = "MakeAllElemental_BoundCreature_Rune",
        menuName = "Abilities/Runes/MakeAllElemental_BoundCreature_Rune")]
    public class MakeAllElemental_BoundCreature_Rune : PassiveAbilitySO
    {
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            // can do this because execute on place
            CreatureStats creatureStats = GetCreatureObjFromEventDataTile(eventData).GetComponent<CreatureStats>();

            creatureStats.element = CreatureDataSO.Element.Any;
        }
    
        public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
        {
            CreatureStats boundCreatureStats = thisCard.GetCreatureStats_FromBoundRune_Ext();

            if (boundCreatureStats == null)
            {
                Debug.LogError($"Creature bound by this rune ({thisCard}) was found null");
                return;
            }

            // look into the creatures data...
            CreatureDataSO data = boundCreatureStats.cardData as CreatureDataSO;

            //... for there element...
            CreatureDataSO.Element originalElement = data.element;
        
            // and set it back
            boundCreatureStats.element = originalElement;
        }
    }
}
