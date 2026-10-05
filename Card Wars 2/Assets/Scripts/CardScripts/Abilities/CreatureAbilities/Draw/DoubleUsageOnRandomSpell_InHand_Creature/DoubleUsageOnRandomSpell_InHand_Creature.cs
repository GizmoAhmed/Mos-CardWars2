using System.Collections.Generic;
using AbilityEvents;
using CardScripts.CardStats_Folder;
using Extensions;
using UnityEngine;

namespace CardScripts.Abilities.CreatureAbilities.Draw.DoubleUsageOnRandomSpell_InHand_Creature
{
    [CreateAssetMenu(fileName = "DoubleUsageOnRandomSpell_InHand_Creature",
        menuName = "Abilities/Creature/Draw/DoubleUsageOnRandomSpell_InHand_Creature")]
    public class DoubleUsageOnRandomSpell_InHand_Creature : ActiveAbilitySO
    {
        public int usageRate;

        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            // get all spells in hand
            List<SpellStats> spells = thisCard.Ext_GetOwningCardTracker().Server_GetThisPlayerInHandSpells();

            if (spells == null || spells.Count == 0)
            {
                return;
            }

            SpellStats spellChosen = spells.GetRandomFromList();

            if (usageRate > 1)
            {
                spellChosen.uses *= usageRate;
                
                AnimateAbilityExecute(spellChosen.gameObject);
                AnimateAbilityExecute(thisCard);
            }
            else
            {
                Debug.LogError($"Usage Rate variable on {this} ({thisCard.name} is unusable as it's <= 1)");
            }
        }
    }
}
