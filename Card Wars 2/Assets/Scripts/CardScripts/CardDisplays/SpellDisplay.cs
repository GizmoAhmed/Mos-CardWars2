using CardScripts.CardData;
using CardScripts.CardStats_Folder;
using UnityEngine;

namespace CardScripts.CardDisplays
{
    public class SpellDisplay : CardDisplay
    {
        private GameObject usesUIObject;
        private GameObject xUIObject;
        
        private void Awake()
        {
            FindDisplayParts();
        }

        protected override void FindDisplayParts()
        {
            base.FindDisplayParts();
            
            usesUIObject = FindPart("Uses");
            xUIObject = FindPart("x");

            if (usesUIObject == null)
            {
                Debug.LogError($"The Uses UI object is null on {gameObject.name}. Make sure to set it in the <color=blue>inspector</color>");
            }

            if (xUIObject == null)
            {
                Debug.LogError($"The little x object is null on {gameObject.name}. Make sure to set it in the <color=blue>inspector</color>");
            }
        }

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