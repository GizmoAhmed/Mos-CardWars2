using System.Collections.Generic;
using AbilityEvents;
using CardScripts.CardStatss;
using Extensions;
using Unity.VisualScripting;
using UnityEngine;

namespace CardScripts.Abilities.CharmAbilities.FortifyAllYourCreatures_OnYourMagicUpgrades_Charm
{
    [CreateAssetMenu(fileName = "FortifyAllYourCreatures_OnYourMagicUpgrades_Charm",
        menuName = "Abilities/Charm/FortifyAllYourCreatures_OnYourMagicUpgrades_Charm")]
    public class FortifyAllYourCreatures_OnYourMagicUpgrades_Charm : PassiveAbilitySO
    {
        public int fortify;
        
        public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
        {
            if(fortify < 0)
            {
                Debug.LogError($"{name} on {thisCard.name}: Fortify amount is not set. Execute will run, but not fortification will be noticeable");
            }

            GameObject thisPlayer = thisCard.Ext_GetOwningPlayerStats().gameObject;

            // ask if this card's owner was the one that upgraded
            if (thisPlayer == eventData.target) // yes
            {
                // Debug.Log($"{name}: <color=teal>Matching Player!</color>");
                
                // get all your creatures
                List<CreatureStats> allYourCreatures = thisCard.Ext_GetAllActiveCreaturesForThisPlayer();

                // fortify them all
                foreach (CreatureStats creature in allYourCreatures)
                {
                    creature.UpdateCreatureDefense(amount: fortify, buff: true);
                }
                AnimateAbilityExecute(thisCard);
            }
            else
            {
                Debug.Log($"{name}: <color=orange>Not your player...</color>");
            }
        }
    }
}