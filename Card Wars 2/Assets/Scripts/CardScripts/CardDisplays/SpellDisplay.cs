using CardScripts.CardData;
using CardScripts.CardStats_Folder;
using UnityEngine;

namespace CardScripts.CardDisplays
{
    public class SpellDisplay : CardDisplay
    {
        [Header("Spell UI Objects")]
        public GameObject usesUIObject;
        public GameObject xUIObject;

        public override void SetDisplayElements_UsingData(CardStats s)
        {
            base.SetDisplayElements_UsingData(s);
            
            // spell specific stuff: 
            
            SpellDataSO spellData = s.CardData as SpellDataSO;

            if (spellData == null)
            {
                Debug.LogError($"SpellData so is null on {gameObject.name}");
                return;
            }

            UpdateUIUses(spellData.uses);
        }

        protected override void ShowCardFlip(bool up)
        {
            base.ShowCardFlip(up);
            
            usesUIObject.SetActive(up);
            xUIObject.SetActive(up);
        }

        public void UpdateUIUses(int newUses)
        {
            SetText(usesUIObject, newUses.ToString());
        }
    }
}