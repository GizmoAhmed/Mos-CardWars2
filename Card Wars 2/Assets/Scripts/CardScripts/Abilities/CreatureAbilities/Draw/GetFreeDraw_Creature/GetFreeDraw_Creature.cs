using AbilityEvents;
using Extensions;
using PlayerStuff;
using UnityEngine;

namespace CardScripts.Abilities.CreatureAbilities.Draw.GetFreeDraw_Creature
{
    [CreateAssetMenu(fileName = "GetFreeDraw_Creature", menuName = "Abilities/Creature/Draw/GetFreeDraw_Creature")]
    public class GetFreeDraw_Creature : ActiveAbilitySO
    {
        public int freeDrawsGained;
        
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            PlayerStats thisPlayer = thisCard.Ext_GetOwningPlayerStats();

            if (freeDrawsGained <= 0)
            {
                Debug.LogWarning($"Floop on {thisCard.name} ({name}) is attempting to give less than 1 free draw");
            }

            thisPlayer.freeDrawsLeft += freeDrawsGained;
        }

    }
}
